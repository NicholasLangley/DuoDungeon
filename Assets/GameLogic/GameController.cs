using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Map loading
    Map map;
    MapBuilder mapBuilder;
    [SerializeField]
    BlockList blockList;
    BlockBuilder blockBuilder;
    //TEMPORARY//
    [SerializeField]
    TextAsset testMap;

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

    [Header("Player Prefabs")]
    [SerializeField]
    PlayerEntity redPlayerPrefab;
    [SerializeField]
    PlayerEntity bluePlayerPrefab;

    List<Entity> _enemies;
    [Header("Enemy Prefabs")]
    [SerializeField]
    Entity enemy1Prefab;

    List<Entity> _enivornmentalEntities;
    [Header("Environmental Entity Prefabs")]
    [SerializeField]
    Entity Evironment1Prefab;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = new PlayerController(Instantiate(redPlayerPrefab), Instantiate(bluePlayerPrefab));

        _enemies = new List<Entity>();
        _enivornmentalEntities = new List<Entity>();

        takingTurn = false;
        undoingTurn = false;
        undoCommandIsBuffered = false;

        previousTurns = new Stack<Turn>();

        //initialize map
        LoadLevel(testMap);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            undoCommandIsBuffered = true;
            if(takingTurn)
            {
                previousTurns.Push(currentTurn);
                takingTurn = false;
            }
        }

        //no need to check for turn logic if entities are performing an action
        if (checkForBusyEntities() == true) { return; }
        if (!undoingTurn &&Time.timeScale != 1.0f) { Time.timeScale = 1.0f; }

        //interrupt infinite loops or long sequences if the player chooses
        if (undoCommandIsBuffered == true)
        {
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
            currentlyUndoingTurn.undoLayer(currentUndoLayer--);
            if (currentUndoLayer < 0) { FinishUndoingTurn(); }
        }

        else { checkForPlayerTakingTurn(); }


    }

    ///////////////////
    ///Level Loading///
    ///////////////////
    public void LoadLevel(TextAsset level)
    {
        map = new Map();
        blockBuilder = new BlockBuilder(blockList);
        mapBuilder = new MapBuilder(blockBuilder, map);

        //blocks only
        mapBuilder.LoadMap(level);

        //spawn Players
        playerController.SpawnPlayers(mapBuilder.redPlayerSpawn, mapBuilder.redPlayerSpawnRotation, mapBuilder.bluePlayerSpawn, mapBuilder.bluePlayerSpawnRotation, map);

    }
    

    /////////////////
    //Taking Turn ///
    ////////////////
    
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

    //environmental active triggers such as traps
    List<Command> ActiveEnvironmentalTurn()
    {
        List<Command> triggeredCommands = new List<Command>();

        foreach (Entity environmentalEntity in _enivornmentalEntities)
        {
            Command cmd = environmentalEntity.GetCommand();
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

        return foundCommands;
    }

    /////////////////
    /// Undo Turn ///
    /////////////////
    void BeginUndoingTurn()
    {
        if (previousTurns.Count > 0)
        {
            SetEntitiesToUndo();
            undoingTurn = true;
            currentlyUndoingTurn = previousTurns.Pop();
            currentUndoLayer = currentlyUndoingTurn.TurnActions.Count - 1;
            Time.timeScale = 2.0f;
        }
    }

    void SetEntitiesToUndo()
    {
        playerController.SetPlayersToUndo();

        foreach (Entity enemy in _enemies)
        {
            enemy.currentlyUndoing = true;
        }
    }

    void UnsetEntitiesFromUndo()
    {
        playerController.UnsetPlayersFromUndo();

        foreach (Entity enemy in _enemies)
        {
            enemy.currentlyUndoing = false;
        }
    }

    void FinishUndoingTurn()
    {
        UnsetEntitiesFromUndo();
        undoingTurn = false;
    }


    ////////////////////////
    /// Helper Functions ///
    ////////////////////////
    bool checkForBusyEntities()
    {
        if (playerController.CheckIfBusy() == true) { return true; }
        foreach (Entity enemy in _enemies)
        {
            if (enemy.busy == true) { return true; }
        }
        foreach (Entity environmentalEntity in _enivornmentalEntities)
        {
            if (environmentalEntity.busy == true) { return true; }
        }
        return false;
    }
     
}
