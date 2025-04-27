using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map
{
    Dictionary<Vector3Int, Block> currentStaticBlocks;
    List<GameObject> currentMoveableBlocks;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    public Map()
    {
        currentStaticBlocks = new Dictionary<Vector3Int, Block>();
        currentMoveableBlocks = new List<GameObject>();
    }

    public void ClearMap()
    {
        foreach (Block block in currentStaticBlocks.Values)
        {
            GameObject.Destroy(block.gameObject);
        }
        currentStaticBlocks.Clear();

        foreach (GameObject obj in currentMoveableBlocks)
        {
            GameObject.Destroy(obj);
        }
        currentMoveableBlocks.Clear();
    }


    #region static blocks
    public void AddStaticBlock(Vector3Int position, Block block)
    {
        //overwrite
        if (currentStaticBlocks.ContainsKey(position)) 
        {
            GameObject.Destroy(currentStaticBlocks[position].gameObject);
            currentStaticBlocks.Remove(position); 
        }

        currentStaticBlocks.Add(position, block);
    }

    public void RemoveStaticBlock(Vector3Int position)
    {
        if (currentStaticBlocks.ContainsKey(position))
        {
            GameObject.Destroy(currentStaticBlocks[position].gameObject);
            currentStaticBlocks.Remove(position);
        }
    }

    public Block GetStaticBlock(Vector3Int position)
    {
        if (currentStaticBlocks.ContainsKey(position)) { return currentStaticBlocks[position]; }

        return null;
    }

    public Block GetStaticBlock(Vector3 position)
    {
        return (GetStaticBlock(Map.GetIntVector3(position)));
    }
    #endregion

    #region movingBlocks

    public List<GameObject> GetMovableBlocksList()
    {
        return currentMoveableBlocks;
    }

    public void AddMoveableBlock(GameObject obj)
    {
        currentMoveableBlocks.Add(obj);
    }

    public GameObject CheckGridForDynamicBlock(Vector3 position)
    {
        return CheckGridForDynamicBlock(Map.GetIntVector3(position));
    }

    public GameObject CheckGridForDynamicBlock(Vector3Int positiom)
    {

        return null;
    }

    #endregion

    public Block GetCurrentlyOccupiedBlock(Vector3 gridPosition, DownDirection downDir)
    {
        switch (downDir)
        {
            case DownDirection.Ydown:
                gridPosition.y = Mathf.Floor(gridPosition.y + 0.01f);
                break;
            case DownDirection.Yup:
                gridPosition.y = Mathf.Ceil(gridPosition.y - 0.01f);
                break;
            case DownDirection.Xleft:
                gridPosition.x = Mathf.Floor(gridPosition.x + 0.01f);
                break;
            case DownDirection.Xright:
                gridPosition.x = Mathf.Ceil(gridPosition.x - 0.01f);
                break;
            case DownDirection.Zback:
                gridPosition.z = Mathf.Floor(gridPosition.z + 0.01f);
                break;
            case DownDirection.Zforward:
                gridPosition.z = Mathf.Ceil(gridPosition.z - 0.01f);
                break;
        }

        return (GetStaticBlock(GetIntVector3(gridPosition)));
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
        string filepath = Application.streamingAssetsPath + "/" + filename + ".json";

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
        foreach (Block block in currentStaticBlocks.Values)
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
        json += "\"list_id\": \"" + b.listID + "\",\n";
        json += "\"base_id\": \"" + b.baseID + "\",\n";
        json += "\"varient_id\": \"" + b.varientID + "\",\n";
        //deprecate this, remove once no longer used
        //json += "\"block_id\": " + b.blockID + ",\n";

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
