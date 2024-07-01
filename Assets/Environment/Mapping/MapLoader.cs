using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapLoader
{
    GameObject mapParent;

    BlockBuilder blockBuilder;

    List<GameObject> currentBlocks;

    public MapLoader()
    {
        blockBuilder = new BlockBuilder();
        currentBlocks = new List<GameObject>();
    }

    public void loadMap(TextAsset mapJsonFile)
    {
        //load all prefabs - probably horrible efficiency, optimize later if necessary
        blockBuilder.loadAllPrefabs();


        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJsonFile.text);

        //TODO METADATA STUFF//


        mapParent = new GameObject();
        mapParent.name = "MAP";

        //read and create blocks
        foreach (JObject blockObject in mapJson["blocks"])
        {
        GameObject newBlock = blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]);
        newBlock.transform.parent = mapParent.transform;
        currentBlocks.Add(newBlock);
        }

        blockBuilder.clearAllPrefabs();
    }

    public void clearMap()
    {
        foreach(GameObject block in currentBlocks)
        {
            GameObject.Destroy(block);
        }
        GameObject.Destroy(mapParent);
    }
}
