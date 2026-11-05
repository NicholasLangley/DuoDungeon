using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class MapBuilder
{
    
    //note: all blocks are saved / loaded with their adjusted halfGrid positions and this will need to be converted to world positions on instantiation
    Map map;
    //list IDs for blocks that aren't static in the grid
    List<string> ComplexBlockListIDs = new List<string> { "Pushable" };

    public MapBuilder(Map m)
    {
        map = m;
    }

    public void LoadMap(string filepath)
    {
        map.ClearMap();
        string mapJSON = File.ReadAllText(filepath);
        //map parsing starts here
        JObject mapJson = JObject.Parse(mapJSON);

        //TODO maybe other METADATA STUFF//
        //change load function to match compatible versions
        //TODO maybe clean up old versions if I deprecate them
        switch ((string)mapJson["metadata"]["saveVersion"])
        {
            case "0.25":
                LoadMapVersion0_25(mapJson);
                break;
            default:
                //TODO error message to user
                Debug.Log("Unknown map format: " + mapJson["metadata"]["saveVersion"]);
                return;
        }

        //update map to newest version (backup of last version will be made / overwritten)
        if ((string)mapJson["metadata"]["saveVersion"] != Map._currentNewSaveVersion)
        {
            Debug.Log("updating map from version " + (string)mapJson["metadata"]["saveVersion"] + " to version " + Map._currentNewSaveVersion);
            map.SaveMapToFile(filepath);
        }
        //map.DebugMap();
    }

    void LoadMapVersion0_25(JObject mapJson)
    {
        LoadMetadata(mapJson);
        LoadMapBlocks(mapJson);
        LoadPlayerSpawns((JObject)mapJson["players"]);
    }

    void LoadMetadata(JObject mapJson)
    {
        //todo do we need to do anything anything else with the metadata? author(s)?
        map.mapName = mapJson["metadata"]["name"].ToString();
        map.version = (int)mapJson["metadata"]["version"];
    }

    void LoadMapBlocks(JObject mapJson)
    {
        //todo refactor to flyweight pattern for rendering
        foreach (KeyValuePair<string, JToken> blockTemplate in (JObject)mapJson["gridBlocks"])
        {
            PlaceableKey key = new PlaceableKey(blockTemplate.Key);
            
            BlockData blockData = map.LoadBlockData(key.listID, key.baseID, key.varientID);
            if (blockData == null)
            {
                //TODO log to file / error message user?
                Debug.Log("WARNING: blockData for  " + key.getKeyString() +  " not found");
                continue;
            }
            
            JArray positionsArray = (JArray)blockTemplate.Value;
            
            foreach (JToken position in positionsArray)
            {
                JArray positionArray = (JArray)position;
                
                Vector3Int halfGridPos = new Vector3Int((int)positionArray[0],  (int)positionArray[1], (int)positionArray[2]);
                BlockInstance block = new BlockInstance(blockData, new Quaternion(key.rotationX, key.rotationY, key.rotationZ, key.rotationW));
                map.AddBlockToHalfGridPos(halfGridPos, block);
                
                
                //DEPRECATED
                /*
                GameObject newBlockObject = blockBuilder.BuildBlock(key, (int)positionArray[0],  (int)positionArray[1], (int)positionArray[2]);
                
                Vector3Int worldPosition = Map.GetIntVector3(newBlockObject.transform.position);
                PlaceableMonoBehaviour placeableMonoBehaviour = newBlockObject.GetComponent<PlaceableMonoBehaviour>();
                if (ComplexBlockListIDs.Contains(placeableMonoBehaviour.listID)) { map.AddComplexBlock(newBlockObject.GetComponent<ComplexBlock>()); }
                else { map.AddBlockToWorldPos(worldPosition, newBlockObject.GetComponent<BlockOLD>()); newBlockObject.GetComponent<BlockOLD>().SetPosition(worldPosition); }
                newBlockObject.transform.parent = mapParent.transform;*/
            }
        }
    }
    
    private void LoadPlayerSpawns(JObject playerJObject)
    {
        map.redPlayerSpawn = parseJSONPosition(playerJObject["red"]["position"]);
        map.redPlayerSpawnRotation = ParseJSONRotation(playerJObject["red"]["rotation"]);

        map.bluePlayerSpawn = parseJSONPosition(playerJObject["blue"]["position"]);
        map.bluePlayerSpawnRotation = ParseJSONRotation(playerJObject["blue"]["rotation"]);
    }
    
    public static Vector3 parseJSONPosition(JToken pos)
    {
        return new Vector3((int)pos["x"], (int)pos["y"], (int)pos["z"]);
    }
    
    public static Quaternion ParseJSONRotation(JToken rotation)
    {
        return new Quaternion((float)rotation["x"], (float)rotation["y"], (float)rotation["z"], (float)rotation["w"]);
    }

    
    /*todo deprecate
    void LoadMapVersion0_1(JObject mapJson)
    {
        //read and create blocks
        //todo refactor to:
        //use flyweight pattern
        //use more compact save file structure hierarchy
        //- block ids
        //-- rotations
        //--- block specific (positions, etc.)
        foreach (JObject blockObject in mapJson["blocks"])
        {
            GameObject newBlockObject = blockBuilder.BuildBlockOLD(blockObject["info"]);
            //block already converted to world in BlockBuilder
            Vector3Int worldPosition = Map.GetIntVector3(newBlockObject.transform.position);
            PlaceableMonoBehaviour placeableMonoBehaviour = newBlockObject.GetComponent<PlaceableMonoBehaviour>();
            if (ComplexBlockListIDs.Contains(placeableMonoBehaviour.listID)) { map.AddComplexBlock(newBlockObject.GetComponent<ComplexBlock>()); }
            else { map.AddBlockToWorldPos(worldPosition, newBlockObject.GetComponent<BlockOLD>()); newBlockObject.GetComponent<BlockOLD>().SetPosition(worldPosition); }
            newBlockObject.transform.parent = mapParent.transform;
        }

        //readPlayers
        LoadPlayerSpawns((JObject)mapJson["players"]);
    }*/
}
