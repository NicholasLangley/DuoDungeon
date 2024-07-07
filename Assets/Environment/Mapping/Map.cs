using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    Dictionary<Vector3Int, Block> currentBlocks;

    public void Start()
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

    public Block GetBlock(Vector3Int position)
    {
        if (currentBlocks.ContainsKey(position)) { return currentBlocks[position]; }

        return null;
    }

    public static Vector3Int GetIntVector3(Vector3 floatVector)
    {
        return new Vector3Int(Mathf.RoundToInt(floatVector.x), Mathf.RoundToInt(floatVector.y), Mathf.RoundToInt(floatVector.z));
    }


    public void saveMapToFile(string filename)
    {
        string mapJSON = "{\n";

        //METADATA
        mapJSON += "\"metadata\": \"EMPTY\",\n";

        //start of blocks array
        mapJSON += "\"blocks\": [\n";

        bool firstBlock = true;
        //fill blocks array
        foreach (Block block in currentBlocks.Values)
        {
            //add comma after previous block
            if (!firstBlock) { mapJSON += ",\n"; }
            else { firstBlock = false; }
            mapJSON += convertBlockToJSON(block);
        }

        //finish blocks array and file
        mapJSON += "\n]\n}";

        Debug.Log(mapJSON);
        //save file to text
        string filepath = Application.dataPath + "/" + filename + ".json";

        //overwrites file if it already exists
        File.WriteAllText(filepath, mapJSON);
    }

    string convertBlockToJSON(Block b)
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
        json += "\"position\": {\n" +
            "\"x\": " + b.transform.position.x + ",\n" +
            "\"y\": " + b.transform.position.y + ",\n" +
            "\"z\": " + b.transform.position.z + "\n" +
            "},\n";

        //rotation
        json += "\"rotation\": {\n" +
            "\"w\": " + b.transform.rotation.w + ",\n" +
            "\"x\": " + b.transform.rotation.x + ",\n" +
            "\"y\": " + b.transform.rotation.y + ",\n" +
            "\"z\": " + b.transform.rotation.z + "\n" +
            "}\n";//add comma here if more fields added

        //finish info
        json += "}\n";

        //finish block
        json += "}\n";

        return json;
    }
}
