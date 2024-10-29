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

    /////////////////
    //TEMPORARY///////
    //////////////////
    [SerializeField] TextAsset temporaryLoadMap;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map();
        blockBuilder = new BlockBuilder(blockList);
        mapBuilder = new MapBuilder(blockBuilder, map);
        objectPlacer.map = map;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert))
        {
            map.SaveMapToFile("/Autotests/MovementBlockedBasicTestMap");
            Debug.Log("map saved");
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
            mapBuilder.LoadMap("/Autotests/MovementBlockedBasicTestMap" + ".json");
            //TODO do this better
            objectPlacer.redPlayerPlacementIndicator.transform.position = map.redPlayerSpawn;
            objectPlacer.redPlayerPlacementIndicator.transform.rotation = map.redPlayerSpawnRotation;
            objectPlacer.bluePlayerPlacementIndicator.transform.position = map.bluePlayerSpawn;
            objectPlacer.bluePlayerPlacementIndicator.transform.rotation = map.bluePlayerSpawnRotation;
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
}
