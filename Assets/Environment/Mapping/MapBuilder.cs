using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapBuilder
{
    Map map;
    BlockBuilder blockBuilder;
    GameObject mapParent;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    public MapBuilder(BlockBuilder bb, Map m)
    {
        map = m;
        blockBuilder = bb;

        mapParent = new GameObject("MapParent");
    }

    public void LoadMap(TextAsset mapJsonFile)
    {
        map.ClearMap();
        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJsonFile.text);

        //TODO METADATA STUFF//
        

        //read and create blocks
        foreach (JObject blockObject in mapJson["blocks"])
        {
            Block newBlock = blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]);
            Vector3Int intPosition = Map.GetIntVector3(newBlock.transform.position);
            map.AddBlock(intPosition, newBlock);
            newBlock.transform.parent = mapParent.transform;
        }

        //readPlayers
        ReadPlayers((JObject)mapJson["players"]);

        //map.SaveMapToFile("saveTEST");
    }


    private void ReadPlayers(JObject playerJObject)
    {
        redPlayerSpawn = BlockBuilder.parseJSONPosition(playerJObject["red"]["position"]);
        redPlayerSpawnRotation = BlockBuilder.parseJSONRotation(playerJObject["red"]["rotation"]);

        bluePlayerSpawn = BlockBuilder.parseJSONPosition(playerJObject["blue"]["position"]);
        bluePlayerSpawnRotation = BlockBuilder.parseJSONRotation(playerJObject["blue"]["rotation"]);
    }
}
