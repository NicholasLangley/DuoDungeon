using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //Map loading
    [SerializeField]
    string mapFilename;
    Map map;
    MapBuilder mapBuilder;
    [SerializeField]
    UltimateList ultimateList;
    BlockBuilder blockBuilder;

    //Turn variables
    Stack<Turn> previousTurns;
    bool takingTurn;
    Turn currentTurn;

    bool undoingTurn;
    int currentUndoLayer;
    Turn currentlyUndoingTurn;
    bool undoCommandIsBuffered;

    //turn order
    //1. Player
    //2. Enemies
    //3. Environment
    PlayerController playerController;
    [SerializeField]
    PlayerInputHandler playerInputHandler;

    [Header("Player Prefabs")]
    [SerializeField]
    PlayerEntity redPlayerPrefab;
    [SerializeField]
    PlayerEntity bluePlayerPrefab;

    List<Entity> _enemies;

    [SerializeField]
    List<StaticEnvironmentObject> _staticEnvironmentObjects;
    List<Pushable> _pushableObjects;

    float gameSpeedMultiplier { get; set; } //multiplies all game speeds for testing purposes

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        playerController = new PlayerController(Instantiate(redPlayerPrefab), Instantiate(bluePlayerPrefab), playerInputHandler, this);

        _enemies = new List<Entity>();
        //_staticEnvironmentObjects = new List<StaticEnvironmentObject>();
        _pushableObjects = new List<Pushable>();

        takingTurn = false;
        undoingTurn = false;
        undoCommandIsBuffered = false;

        previousTurns = new Stack<Turn>();

        //initialize map
        string levelString = LevelManager.currentLevelPath;
        if (levelString == null || string.Compare(levelString, "") == 0) { levelString = mapFilename; }
        LoadLevel("/" + levelString + ".json");

        //TEMP FOR TESTING, eventually these will be loaded properly//
        foreach (FullGridMoveable fgm in _pushableObjects)
        {
            fgm.map = map;
            fgm.gameController = this;
        }

        gameSpeedMultiplier = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //update player aim from mouse movement
        playerController.updatePlayerAim();

        if (playerInputHandler.undoInput && !undoingTurn)
        {
            undoCommandIsBuffered = true;
        }

        //no need to check for turn logic if entities are performing an action
        if (checkForBusyEntities() == true) { return; }
        if (!undoingTurn &&Time.timeScale != 1.0f) { Time.timeScale = 1.0f * gameSpeedMultiplier; }

        //interrupt infinite loops or long sequences if the player chooses
        if (undoCommandIsBuffered == true)
        {
            if (takingTurn)
            {
                previousTurns.Push(currentTurn);
                takingTurn = false;
            }
            BeginUndoingTurn();
            undoCommandIsBuffered = false;
        }
        


        else if (takingTurn)
        {
            List<Command> activeEnvironmentCommands = ActiveEnvironmentalTurn();
            List<Command> passiveEnvironmentCommands = PassiveEnvironmentalTurn();
            //ends current turn
            if (activeEnvironmentCommands.Count + passiveEnvironmentCommands.Count <= 0)
            {
                previousTurns.Push(currentTurn);
                takingTurn = false;
            }
            //add commands to turn layer and continue
            else{
                currentTurn.AddTurnLayer();
                foreach(Command activeCMD in activeEnvironmentCommands)
                {
                    currentTurn.AddCommand(activeCMD);
                }
                foreach (Command passiveCMD in passiveEnvironmentCommands)
                {
                    currentTurn.AddCommand(passiveCMD);
                }
            }
        }

        else if (undoingTurn)
        {
            if (currentUndoLayer < 0) { FinishUndoingTurn(); return; }
            currentlyUndoingTurn.undoLayer(currentUndoLayer--);
        }

        else { checkForPlayerTakingTurn(); }

        if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene("FREE-MOVE"); }
        else if (Input.GetKeyDown(KeyCode.G)) { SceneManager.LoadScene("GRID-MOVE"); }
    }

    ///////////////////
    ///Level Loading///
    ///////////////////
    #region Level Loading
    public void LoadLevel(string filename)
    {
        if (map != null) 
        { 
            map.ClearMap();
            _pushableObjects = new List<Pushable>();
        }
        map = new Map();
        blockBuilder = new BlockBuilder(ultimateList);
        mapBuilder = new MapBuilder(blockBuilder, map);

        //blocks only
        mapBuilder.LoadMap(filename);

        List<ComplexBlock> complexBlocks = map.GetComplexBlocksList();
        foreach (ComplexBlock block in complexBlocks)
        {
            Pushable pushableComponent = block.gameObject.GetComponent<Pushable>();
            if (pushableComponent != null)
            {
                _pushableObjects.Add(pushableComponent);
            }
        }

        //spawn Players
        playerController.SpawnPlayers(map);
        playerController.SetPlayerGravity();

    }
    #endregion

    /////////////////
    //Taking Turn ///
    ////////////////
    #region Taking Turn
    //StartTurn on player input
    void checkForPlayerTakingTurn()
    {
        List<Command> playerCommands = playerController.GetCommands();

        //no turn if player does not input
        if (playerCommands == null) { return; }

        takingTurn = true;

        currentTurn = new Turn();

        foreach (Command cmd in playerCommands)
        {
            cmd.Execute();
            currentTurn.AddCommand(cmd);
        }

        ExecutePlayerInteractedObjectsCommands();
        enemiesTakeTurn();
    }

    //enemies act at same time as player
    void enemiesTakeTurn()
    {
        foreach (Entity enemy in _enemies)
        {
            Command cmd = enemy.GetCommand();
            if (cmd != null)
            {
                cmd.Execute();
                currentTurn.AddCommand(cmd);
            }
        }
    }

    //anything the player interacts with during their action
    //such as apushing an object or interacting with a switch
    void ExecutePlayerInteractedObjectsCommands()
    {
        List<Command> interactionCommands = new List<Command>();

        foreach (Pushable pushable in _pushableObjects)
        {
            Command cmd = pushable.GetCommand();
            if (cmd != null)
            {
                cmd.Execute();
                currentTurn.AddCommand(cmd);
            }
        }
    }


    //environmental active triggers such as traps
    List<Command> ActiveEnvironmentalTurn()
    {
        List<Command> triggeredCommands = new List<Command>();

        foreach (StaticEnvironmentObject environmentObject in _staticEnvironmentObjects)
        {
            Command cmd = environmentObject.GetCommand();
            if (cmd != null)
            {
                cmd.Execute();
                triggeredCommands.Add(cmd);
            }
        }

        return triggeredCommands;
    }

    //entities check for active environmental effects such as ice or holes
    //occurs after any traps could have gone off and only affects non busy enemies
    List<Command> PassiveEnvironmentalTurn()
    {
        List<Command> foundCommands = new List<Command>();

        //players
        List<Command> playerFoundCommands = playerController.GetPassiveCommands();
        foreach (Command cmd in playerFoundCommands)
        {
            cmd.Execute();
            foundCommands.Add(cmd);
        }


        //enemies
        foreach (Entity enemy in _enemies)
        {
            if(!enemy.busy)
            {
                Command cmd = enemy.GetPassiveCommand();
                if(cmd != null)
                {
                    cmd.Execute();
                    foundCommands.Add(cmd);
                }
            }
        }

        //pushables
        foreach (Pushable pushable in _pushableObjects)
        {
            if (!pushable.busy)
            {
                Command cmd = pushable.GetPassiveCommand();
                if (cmd != null)
                {
                    cmd.Execute();
                    foundCommands.Add(cmd);
                }
            }
        }

        foreach (StaticEnvironmentObject envObj in _staticEnvironmentObjects)
        {
            if (!envObj.busy)
            {
                Command cmd = envObj.GetPassiveCommand();
                if (cmd != null)
                {
                    cmd.Execute();
                    foundCommands.Add(cmd);
                }
            }
        }

        return foundCommands;
    }
    #endregion

    /////////////////
    /// Undo Turn ///
    /////////////////
    #region Undo Turn
    void BeginUndoingTurn()
    {
        if (previousTurns.Count > 0)
        {
            SetEntitiesToUndo();
            undoingTurn = true;
            currentlyUndoingTurn = previousTurns.Pop();
            currentUndoLayer = currentlyUndoingTurn.TurnActions.Count - 1;
            Time.timeScale = 2.0f * gameSpeedMultiplier;
        }
    }

    void SetEntitiesToUndo()
    {
        playerController.SetPlayersToUndo();

        foreach (Entity enemy in _enemies)
        {
            enemy.currentlyUndoing = true;
        }

        foreach (Pushable pushable in _pushableObjects)
        {
            pushable.currentlyUndoing = true;
        }

        foreach (StaticEnvironmentObject envObj in _staticEnvironmentObjects)
        {
            envObj.currentlyUndoing = true;
        }
    }

    void UnsetEntitiesFromUndo()
    {
        playerController.UnsetPlayersFromUndo();

        foreach (Entity enemy in _enemies)
        {
            enemy.currentlyUndoing = false;
        }

        foreach (Pushable pushable in _pushableObjects)
        {
            pushable.currentlyUndoing = false;
        }

        foreach (StaticEnvironmentObject envObj in _staticEnvironmentObjects)
        {
            envObj.currentlyUndoing = false;
        }
    }

    void FinishUndoingTurn()
    {
        UnsetEntitiesFromUndo();
        undoingTurn = false;
    }
    #endregion


    ////////////////////////
    /// Helper Functions ///
    ////////////////////////
    #region Helper Functions
    bool checkForBusyEntities()
    {
        if (playerController.CheckIfBusy() == true) { return true; }
        foreach (Entity enemy in _enemies)
        {
            if (enemy.busy == true) { return true; }
        }
        foreach(Pushable pushable in _pushableObjects)
        {
            if (pushable.busy == true) { return true; }
        }
        foreach (StaticEnvironmentObject environmentObject in _staticEnvironmentObjects)
        {
            if (environmentObject.busy == true) { return true; }
        }
        return false;
    }

    public FullGridMoveable GetFGMAtPosition(Vector3Int destinationToCheck)
    {
        //check players
        Entity playerEntity = playerController.GetPlayerAtPosition(destinationToCheck);
        if (playerEntity != null) { return playerEntity; }
        
        //check enemies
        foreach(Entity enemy in _enemies)
        {
            if (enemy.GetCurrentBlockPosition() == destinationToCheck) { return enemy; }
        }

        return null;
    }

    public void SetTimeScaleMultiplier(float m)
    {
        Time.timeScale /= gameSpeedMultiplier;
        gameSpeedMultiplier = m;
        Time.timeScale *= gameSpeedMultiplier;
    }
    #endregion


    #region temporary?

    public void SetPlayerClassicDungeonCrawlerMode(bool mode)
    {
        playerController.setClassicDungeonCrawlerMode(mode);
    }

    #endregion
}
