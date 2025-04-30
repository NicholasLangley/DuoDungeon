using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map
{
    Dictionary<Vector3Int, Block> currentStaticBlocks;
    List<ComplexBlock> currentComplexBlocks;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    public Map()
    {
        currentStaticBlocks = new Dictionary<Vector3Int, Block>();
        currentComplexBlocks = new List<ComplexBlock>();
    }

    public void ClearMap()
    {
        foreach (Block block in currentStaticBlocks.Values)
        {
            GameObject.Destroy(block.gameObject);
        }
        currentStaticBlocks.Clear();

        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            GameObject.Destroy(complexBlock.gameObject);
        }
        currentComplexBlocks.Clear();
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

    #region complexBlocks

    public List<ComplexBlock> GetComplexBlocksList()
    {
        return currentComplexBlocks;
    }

    public void AddComplexBlock(ComplexBlock complexBlock)
    {
        currentComplexBlocks.Add(complexBlock);
    }

    public Block CheckGridForComplexBlock(Vector3 position, GameObject requestor)
    {
        return CheckGridForComplexBlock(Map.GetIntVector3(position), requestor);
    }

    public Block CheckGridForComplexBlock(Vector3Int position, GameObject requestor)
    {
        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            if (complexBlock.gameObject != requestor)
            {
                foreach (Block gridBlock in complexBlock.GetGridBlocks())
                {
                    if (gridBlock.gridPosition == position)
                    {
                        return gridBlock;
                    }
                }
            }
        }
        return null;
    }

    #endregion

    public Block GetCurrentlyOccupiedBlock(GameObject requestor, DownDirection downDir)
    {
        Vector3 gridPosition = requestor.transform.position;
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

        Block complexBlock = CheckGridForComplexBlock(gridPosition, requestor);
        if (complexBlock != null ) { return complexBlock; }
        return (GetStaticBlock(GetIntVector3(gridPosition)));
    }

    //assumes input vector is very close to int already, and rounds to int to remove any floating point differences from small errors in transfrom positioning 
    //DO NOT USE TO DETERMINE WHICH GRID SPACE AN OFFSET BLOCK IS IN AS THIS MAY ROUND UP
    public static Vector3Int GetIntVector3(Vector3 floatVector)
    {
        return new Vector3Int(Mathf.RoundToInt(floatVector.x), Mathf.RoundToInt(floatVector.y), Mathf.RoundToInt(floatVector.z));
    }

    //Rounds to floor to get grid space vector is a part of
    public static Vector3Int GetGridSpace(Vector3 floatVector)
    {
        return new Vector3Int(Mathf.FloorToInt(floatVector.x), Mathf.FloorToInt(floatVector.y), Mathf.FloorToInt(floatVector.z));
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
