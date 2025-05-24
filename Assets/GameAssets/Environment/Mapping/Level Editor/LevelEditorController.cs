using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour
{
    //Map building
    [SerializeField]
    string mapFilename;
    [SerializeField]
    Map map;
    MapBuilder mapBuilder;
    [SerializeField]
    UltimateList ultimateList;
    [SerializeField]
    BlockSelector blockSelector;
    BlockBuilder blockBuilder;

    [SerializeField]
    ObjectPlacer objectPlacer;

    //Command Stacks
    Stack<Turn> undoTurns;
    Stack<Turn> redoTurns;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map();
        blockBuilder = new BlockBuilder(ultimateList);
        mapBuilder = new MapBuilder(blockBuilder, map);
        objectPlacer.map = map;
        objectPlacer.SetBlockList(ultimateList);

        undoTurns = new Stack<Turn>();
        redoTurns = new Stack<Turn>();

        //TODO populate multiple lists
        blockSelector.populateButtons(ultimateList.GetMasterList("Basic Block"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Load();
        }

        //temporary player selection
        else if (Input.GetKeyDown(KeyCode.V))
        {
            objectPlacer.SetPlayer(true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            objectPlacer.SetPlayer(false);
        }
    }

    public void AddCommandTurn(List<Command> commands)
    {
        if (commands == null) { return; }
        Turn turn = new Turn();
        foreach(Command cmd in commands)
        {
            turn.AddCommand(cmd);
        }
        undoTurns.Push(turn);
        redoTurns.Clear();
    }

    public void Undo()
    {
        if (undoTurns.Count > 0)
        {
            Turn undoneTurn = undoTurns.Pop();
            undoneTurn.undoLayer(0);
            redoTurns.Push(undoneTurn);
        }
    }

    public void Redo()
    {
        if (redoTurns.Count > 0)
        {
            Turn redoTurn = redoTurns.Pop();
            redoTurn.redoLayer(0);
            undoTurns.Push(redoTurn);
        }
    }

    public void Save()
    {
        map.SaveMapToFile("/" + mapFilename);
        Debug.Log("map saved");
    }

    public void Load()
    {
        string levelString = LevelManager.currentLevelPath;
        if (levelString == null || string.Compare(levelString, "") == 0) { levelString = mapFilename; }
        mapBuilder.LoadMap("/" + levelString + ".json");
        //TODO do this better
        objectPlacer.redPlayerPlacementIndicator.transform.position = map.redPlayerSpawn;
        objectPlacer.redPlayerPlacementIndicator.transform.rotation = map.redPlayerSpawnRotation;
        objectPlacer.bluePlayerPlacementIndicator.transform.position = map.bluePlayerSpawn;
        objectPlacer.bluePlayerPlacementIndicator.transform.rotation = map.bluePlayerSpawnRotation;
    }
}
