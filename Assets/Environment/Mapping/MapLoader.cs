using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapLoader
{
    BlockBuilder blockBuilder;

    List<GameObject> currentBlocks;

    public MapLoader()
    {
        blockBuilder = new BlockBuilder();
        currentBlocks = new List<GameObject>();
    }

    public void loadMap(TextAsset mapJsonFile)
    {
        JObject mapJson = JObject.Parse(mapJsonFile.text);
        
        //TODO METADATA STUFF//

        //read blocks
        foreach(JObject blockObject in mapJson["blocks"])
        {
            currentBlocks.Add(blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]));
        }
    }

    public void clearMap()
    {
        foreach(GameObject block in currentBlocks)
        {
            GameObject.Destroy(block);
        }
    }

}
