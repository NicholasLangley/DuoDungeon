using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map
{
    Dictionary<Vector3Int, Block> currentBlocks;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    public Map()
    {
        currentBlocks = new Dictionary<Vector3Int, Block>();
    }

    public void ClearMap()
    {
        foreach (Block block in currentBlocks.Values)
        {
            GameObject.Destroy(block.gameObject);
        }
        currentBlocks.Clear();
    }

    public void AddBlock(Vector3Int position, Block block)
    {
        //overwrite
        if (currentBlocks.ContainsKey(position)) 
        {
            GameObject.Destroy(currentBlocks[position].gameObject);
            currentBlocks.Remove(position); 
        }

        currentBlocks.Add(position, block);
    }

    public void RemoveBlock(Vector3Int position)
    {
        if (currentBlocks.ContainsKey(position))
        {
            GameObject.Destroy(currentBlocks[position].gameObject);
            currentBlocks.Remove(position);
        }
    }

    public Block GetBlock(Vector3Int position)
    {
        if (currentBlocks.ContainsKey(position)) { return currentBlocks[position]; }

        return null;
    }

    public Block GetBlock(Vector3 position)
    {
        return (GetBlock(Map.GetIntVector3(position)));
    }

    public static Vector3Int GetIntVector3(Vector3 floatVector)
    {
        return new Vector3Int(Mathf.RoundToInt(floatVector.x), Mathf.RoundToInt(floatVector.y), Mathf.RoundToInt(floatVector.z));
    }


    public void SaveMapToFile(string filename)
    {
        if(redPlayerSpawn == null || redPlayerSpawnRotation == null || bluePlayerSpawn == null || bluePlayerSpawnRotation == null)
        {
            //TODO allow this but mark map as incomplete/unplayable
            Debug.Log("ERROR missing players from map, cannot save as is");
            return;
        }

        string mapJSON = "{\n";

        //METADATA
        mapJSON += "\"metadata\": \"EMPTY\",\n";

        //Players
        mapJSON += GetPlayersJSON();

        //Blocks Data
        mapJSON += GetBlocksListJSON();

        //end file
        mapJSON += "\n}";

        Debug.Log(mapJSON);
        //save file to text
        string filepath = Application.dataPath + "/levelsTEMP/" + filename + ".json";

        //overwrites file if it already exists
        File.WriteAllText(filepath, mapJSON);
    }

    string GetBlocksListJSON()
    {
        string blocksListJSON = "";

        //start of blocks array
        blocksListJSON += "\"blocks\": [\n";

        bool firstBlock = true;
        //fill blocks array
        foreach (Block block in currentBlocks.Values)
        {
            //add comma after previous block
            if (!firstBlock) { blocksListJSON += ",\n"; }
            else { firstBlock = false; }
            blocksListJSON += ConvertBlockToJSON(block);
        }

        //finish blocks array
        blocksListJSON += "\n]";

        return blocksListJSON;
    }

    string GetPlayersJSON()
    {
        return "\"players\": {\n" +
                    "\"red\": {\n" +
                        GetPositionJSON(redPlayerSpawn) +
                        GetRotationJSON(redPlayerSpawnRotation) +
                    "\n},\n"    +
                    "\"blue\": {\n" +
                        GetPositionJSON(bluePlayerSpawn) +
                        GetRotationJSON(bluePlayerSpawnRotation) +
              "}\n},\n";
    }

    string ConvertBlockToJSON(Block b)
    {
        //start block
        string json = "{\n";

        //block type TODO
        json += "\"type\": 0,\n";

        //INFO
        json += "\"info\": {\n";

        //blockID
        json += "\"block_id\": " + b.blockID + ",\n";

        //position
        json += GetPositionJSON(b.transform.position);

        //rotation
        json += GetRotationJSON(b.transform.rotation); //add comma to this function if adding more parts to block info

        //finish info
        json += "}\n";

        //finish block
        json += "}\n";

        return json;
    }

    string GetPositionJSON(Vector3 pos)
    {
        return "\"position\": {\n" +
            "\"x\": " + pos.x + ",\n" +
            "\"y\": " + pos.y + ",\n" +
            "\"z\": " + pos.z + "\n" +
            "},\n";
    }

    string GetRotationJSON(Quaternion rot)
    {
        return "\"rotation\": {\n" +
            "\"w\": " + rot.w + ",\n" +
            "\"x\": " + rot.x + ",\n" +
            "\"y\": " + rot.y + ",\n" +
            "\"z\": " + rot.z + "\n" +
            "}\n";//add comma here if more fields added to block ie, components
    }
}
