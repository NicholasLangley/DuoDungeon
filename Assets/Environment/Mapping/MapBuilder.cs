using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapBuilder
{
    Map map;
    BlockBuilder blockBuilder;

    public MapBuilder(BlockBuilder bb, Map m)
    {
        map = m;
        blockBuilder = bb;
    }

    public void loadMap(TextAsset mapJsonFile)
    {
        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJsonFile.text);

        //TODO METADATA STUFF//

        //read and create blocks
        foreach (JObject blockObject in mapJson["blocks"])
        {
            Block newBlock = blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]);
            Vector3Int intPosition = Map.GetIntVector3(newBlock.transform.position);
            map.AddBlock(intPosition, newBlock);
        }
    }

    public void SaveMap(string path)
    {

    }

}
