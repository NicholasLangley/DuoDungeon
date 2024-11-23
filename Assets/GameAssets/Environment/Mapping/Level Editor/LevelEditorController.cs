using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour
{
    //Map building
    [SerializeField]
    Map map;
    MapBuilder mapBuilder;
    [SerializeField]
    BlockList blockList;
    BlockBuilder blockBuilder;

    [SerializeField]
    ObjectPlacer objectPlacer;

    //Command Stacks
    Stack<Command> undoCommands;
    Stack<Command> redoCommands;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map();
        blockBuilder = new BlockBuilder(blockList);
        mapBuilder = new MapBuilder(blockBuilder, map);
        objectPlacer.map = map;

        undoCommands = new Stack<Command>();
        redoCommands = new Stack<Command>();
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

    public void AddCommand(Command cmd)
    {
        if (cmd == null) { return; }
        undoCommands.Push(cmd);
        redoCommands.Clear();
    }

    public void Undo()
    {
        if (undoCommands.Count > 0)
        {
            Command undoneCommand = undoCommands.Pop();
            undoneCommand.Undo();
            redoCommands.Push(undoneCommand);
        }
    }

    public void Redo()
    {
        if (redoCommands.Count > 0)
        {
            Command redoCommand = redoCommands.Pop();
            redoCommand.Execute();
            undoCommands.Push(redoCommand);
        }
    }

    public void Save()
    {
        map.SaveMapToFile("/Autotests/FrontBackOrientationTestMap");
        Debug.Log("map saved");
    }

    public void Load()
    {
        mapBuilder.LoadMap("/Autotests/FrontBackOrientationTestMap" + ".json");
        //TODO do this better
        objectPlacer.redPlayerPlacementIndicator.transform.position = map.redPlayerSpawn;
        objectPlacer.redPlayerPlacementIndicator.transform.rotation = map.redPlayerSpawnRotation;
        objectPlacer.bluePlayerPlacementIndicator.transform.position = map.bluePlayerSpawn;
        objectPlacer.bluePlayerPlacementIndicator.transform.rotation = map.bluePlayerSpawnRotation;
    }
}
