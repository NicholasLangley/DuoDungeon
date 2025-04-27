using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class MapBuilder
{
    Map map;
    BlockBuilder blockBuilder;
    GameObject mapParent;
    //list IDs for blocks that aren't static in the grid
    List<string> moveableBlockListIDs = new List<string> { "Pushable" };

    public MapBuilder(BlockBuilder bb, Map m)
    {
        map = m;
        blockBuilder = bb;

        mapParent = new GameObject("MapParent");
    }

    public void LoadMap(string filename)
    {
        map.ClearMap();
        string filepath = Application.streamingAssetsPath + "/" + filename;
        string mapJSON = File.ReadAllText(filepath);
        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJSON);

        //TODO METADATA STUFF//
        

        //read and create blocks
        foreach (JObject blockObject in mapJson["blocks"])
        {
            GameObject newBlockObject = blockBuilder.BuildBlock((BlockBuilder.BlockType)(int)blockObject["type"], blockObject["info"]);
            Vector3Int intPosition = Map.GetIntVector3(newBlockObject.transform.position);
            IPlaceable placeable = newBlockObject.GetComponent<IPlaceable>();
            if (moveableBlockListIDs.Contains(placeable.listID)) { map.AddMoveableBlock(newBlockObject); }
            else { map.AddStaticBlock(intPosition, newBlockObject.GetComponent<Block>()); }
            newBlockObject.transform.parent = mapParent.transform;
        }

        //readPlayers
        ReadPlayers((JObject)mapJson["players"]);
    }


    private void ReadPlayers(JObject playerJObject)
    {
        map.redPlayerSpawn = BlockBuilder.parseJSONPosition(playerJObject["red"]["position"]);
        map.redPlayerSpawnRotation = BlockBuilder.parseJSONRotation(playerJObject["red"]["rotation"]);

        map.bluePlayerSpawn = BlockBuilder.parseJSONPosition(playerJObject["blue"]["position"]);
        map.bluePlayerSpawnRotation = BlockBuilder.parseJSONRotation(playerJObject["blue"]["rotation"]);
    }
}
