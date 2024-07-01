using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapLoader
{
    BlockBuilder blockBuilder;

    public MapLoader()
    {
        blockBuilder = new BlockBuilder();
    }

    public void loadMap(Map map, TextAsset mapJsonFile)
    {
        //load all prefabs - probably horrible efficiency, optimize later if necessary
        blockBuilder.loadAllPrefabs();


        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJsonFile.text);

        //TODO METADATA STUFF//

        //read and create blocks
        foreach (JObject blockObject in mapJson["blocks"])
        {
        GameObject newBlock = blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]);
        Vector3Int intPosition = new Vector3Int(Mathf.RoundToInt(newBlock.transform.position.x), Mathf.RoundToInt(newBlock.transform.position.y), Mathf.RoundToInt(newBlock.transform.position.z));
        map.AddBlock(intPosition, newBlock);
        }

        blockBuilder.clearAllPrefabs();
    }
}
