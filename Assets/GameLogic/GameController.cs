using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Map loading
    [SerializeField]
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

    //turn order
    //1. Player
    //2. Enemies
    //3. Environment
    [SerializeField]
    PlayerController playerController;
    List<Entity> _enemies;
    List<Entity> _enivornmentalEntities;

    // Start is called before the first frame update
    void Start()
    {
        blockBuilder = new BlockBuilder(blockList);
        mapBuilder = new MapBuilder(blockBuilder, map);

        _enemies = new List<Entity>();
        _enivornmentalEntities = new List<Entity>();

        takingTurn = false;
        undoingTurn = false;

        previousTurns = new Stack<Turn>();

        //initialize map
    }

    // Update is called once per frame
    void Update()
    {
        //no need to check for turn logic if entities are performing an action
        if (checkForBusyEntities() == true) { return; }


        if (takingTurn)
        {
            environmentalTurn();
            if (takingTurn == false) { previousTurns.Push(currentTurn);}
        }

        else if (undoingTurn)
        {
            currentlyUndoingTurn.undoLayer(currentUndoLayer--);
            if (currentUndoLayer < 0) { undoingTurn = false; }
        }

        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BeginUndoingTurn();
        }

        else { checkForPlayerTakingTurn(); }

        //TEMP MAP LOAD TEST
        if(Input.GetKeyDown(KeyCode.B))
        {
            map.ClearMap();
            mapBuilder.loadMap(testMap);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            map.ClearMap();
        }
    }

    /////////////////
    //Taking Turn ///
    ////////////////
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

    void environmentalTurn()
    {
        bool environmentalEntityActivatedThisLayer = false;

        foreach (Entity environmentalEntity in _enivornmentalEntities)
        {
            Command cmd = environmentalEntity.GetCommand();
            if (cmd != null)
            {
                if(environmentalEntityActivatedThisLayer == false)
                {
                    currentTurn.AddTurnLayer();
                    environmentalEntityActivatedThisLayer = true;
                }
                cmd.Execute();
                currentTurn.AddCommand(cmd);
            }
        }

        takingTurn = environmentalEntityActivatedThisLayer;
    }

    /////////////////
    /// Undo Turn ///
    /////////////////
    void BeginUndoingTurn()
    {
        if (previousTurns.Count > 0)
        {
            undoingTurn = true;
            currentlyUndoingTurn = previousTurns.Pop();
            currentUndoLayer = currentlyUndoingTurn.TurnActions.Count - 1;
        }
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
