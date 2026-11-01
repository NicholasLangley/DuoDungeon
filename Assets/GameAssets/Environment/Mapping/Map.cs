using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

/*key for save file json
 listID,baseID,varientID,rotation(w,x,y,z)
save file is format
[
  key: [positions[x,y,z]]
  ...
]
*/
public struct PlaceableKey
{
    //properties
    public string listID { get; set; }
    public string baseID { get; set; }
    public string varientID { get; set; }
    public float rotationW { get; set; }
    public float rotationX { get; set; }
    public float rotationY { get; set; }
    public float rotationZ { get; set; }

    public PlaceableKey(string list, string @base, string varient, Quaternion rotation)
    {
        listID = list;
        baseID = @base;
        varientID = varient;
        rotationW = rotation.w;
        rotationX = rotation.x;
        rotationY = rotation.y;
        rotationZ = rotation.z;
    }

    public PlaceableKey(string keyString)
    {
        List<string> keys = new List<String> (keyString.Split(','));
        listID = keys[0];
        baseID = keys[1];
        varientID = keys[2];
        rotationW = float.Parse(keys[3]);
        rotationX = float.Parse(keys[4]);
        rotationY = float.Parse(keys[5]);
        rotationZ = float.Parse(keys[6]);
    }
    
    public string getKeyString()
    {
        return listID + "," + baseID + "," + varientID + "," + rotationW + "," + rotationX + "," + rotationY + "," + rotationZ;
    }

    public void parseKeyString(string keyString)
    {
        List<string> keys = new List<String> (keyString.Split(','));
        listID = keys[0];
        baseID = keys[1];
        varientID = keys[2];
        rotationW = float.Parse(keys[3]);
        rotationX = float.Parse(keys[4]);
        rotationY = float.Parse(keys[5]);
        rotationZ = float.Parse(keys[6]);
    }
}

//The map uses a half grid (all postions multiplied by 2)
//This means that even coordinates indicate grid allignment ie a full square
//odd coordinates indicate a position between two grid spaces
//
//All internal map functions will use this half grid position and it should be opaque to outside functions
//This means calling objects will pass in world coordinates,
//that the map class must convert to internal halfGrid positions ASAP
public class Map
{
    Dictionary<Vector3Int, Block> currentStaticBlocks;
    List<ComplexBlock> currentComplexBlocks;
    Dictionary<Vector3Int, Wall> currentWalls;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    //metadata
    public static string _currentNewSaveVersion = "0.25";
    //TODO set these in level editor, maybe keep list of previous editors?
    string mapAuthor = "Nicole Langley" ;
    public string mapName { get; set; }
    public int version { get; set; }
    

    public Map()
    {
        currentStaticBlocks = new Dictionary<Vector3Int, Block>();
        currentComplexBlocks = new List<ComplexBlock>();
        
        version = 0;
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
    public void AddStaticBlock(Vector3Int pos, Block block)
    {
        Vector3Int position = GetIntVector3(WorldToHalfGrid(pos));
        //overwrite
        if (currentStaticBlocks.ContainsKey(position)) 
        {
            GameObject.Destroy(currentStaticBlocks[position].gameObject);
            currentStaticBlocks.Remove(position); 
        }

        currentStaticBlocks.Add(position, block);
    }

    private void RemoveStaticBlock(Vector3Int position)
    {
        if (currentStaticBlocks.ContainsKey(position))
        {
            GameObject.Destroy(currentStaticBlocks[position].gameObject);
            currentStaticBlocks.Remove(position);
        }
    }

    private Block GetStaticBlock(Vector3Int position)
    {
        if (currentStaticBlocks.ContainsKey(position)) { return currentStaticBlocks[position]; }

        return null;
    }

    private Block GetStaticBlock(Vector3 position)
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

    private Block GetComplexBlockPart(Vector3 position, GameObject requestor)
    {
        return GetComplexBlockPart(Map.GetIntVector3(position), requestor);
    }

    private Block GetComplexBlockPart(Vector3Int position, GameObject requestor)
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

    private void RemoveComplexBlockAtLocation(Vector3 position)
    {
        Block blockPart = GetComplexBlockPart(position, null);
        if (blockPart == null) { return; }
        ComplexBlock  complex = blockPart.GetComponentInParent<ComplexBlock>();
        currentComplexBlocks.Remove(complex);
        GameObject.Destroy(complex.gameObject);
    }

    #endregion

    public void RemoveBlockAtLocation(Vector3 pos)
    {
        Vector3 position = WorldToHalfGrid(pos);
        Block block = GetBlockAtGridPosition(position, null, Vector3.down);
        if (block == null){ return; }
        if (block.GetComponentInParent<ComplexBlock>() != null)
        {
            RemoveComplexBlockAtLocation(position);
        }
        else
        {
            RemoveStaticBlock(Map.GetIntVector3(position));
        }
    }

    public Block GetBlockAtGridPosition(Vector3 pos, GameObject requestor, Vector3 downVector)
    {
        return GetBlockAtGridPosition(Map.GetIntVector3(pos), requestor, downVector);
    }

    public Block GetBlockAtGridPosition(Vector3Int pos, GameObject requestor, Vector3 downVector)
    {
        Vector3Int halfGridPos = WorldToHalfGrid(pos);
        Block staticBlock = GetStaticBlock(halfGridPos);
        Block complexBlockPart = GetComplexBlockPart(halfGridPos, requestor);
        
        if (staticBlock == null) { return complexBlockPart; }
        if (complexBlockPart == null) { return staticBlock; }

        //need to find highest block relative to requestor and return that
        if (staticBlock.GetOrientedTopSide(downVector).centerHeight > complexBlockPart.GetOrientedTopSide(downVector).centerHeight) { return staticBlock; }
        return complexBlockPart;
    }

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

        gridPosition = WorldToHalfGrid(gridPosition);
        Block complexBlock = GetComplexBlockPart(gridPosition, requestor);
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
    // adds a small amount to each value to remove chance of floating point error going one grid space lower down
    public static Vector3Int GetGridSpace(Vector3 floatVector)
    {
        return new Vector3Int(Mathf.FloorToInt(floatVector.x + 0.001f), Mathf.FloorToInt(floatVector.y + 0.001f), Mathf.FloorToInt(floatVector.z + 0.001f));
    }

    public static Vector3Int WorldToHalfGrid(Vector3 worldPos)
    {
        return GetIntVector3(worldPos * 2.0f);
    }

    public static Vector3 HalfGridToWorld(Vector3 halfGridPos)
    {
        return halfGridPos * 0.5f;
    }

    //todo user saves should go to Application.persistentDataPath
    public void SaveMapToFile(string filepath)
    {
        if(redPlayerSpawn == null || redPlayerSpawnRotation == null || bluePlayerSpawn == null || bluePlayerSpawnRotation == null)
        {
            //TODO allow this but mark map as incomplete/unplayable
            Debug.Log("ERROR missing players from map, cannot save as is");
            return;
        }
        
        //todo set this in level editor instead
        //TODO maybe actually use this
        mapName = Path.GetFileNameWithoutExtension(filepath);
        
        JObject saveJSON = new JObject();
        saveJSON.Add("metadata", GetMapMetadataJSON());
        saveJSON.Add("players", GetMapPlayersJSON());
        saveJSON.Add("gridBlocks", GetMapGridBlocksJSON());
        saveJSON.Add("decor", GetMapDecorJSON());
        saveJSON.Add("entities", GetMapEntitiesJSON());
        
        //todo backups before overwrite
        //todo set filepath better. also use the persistantDataPath for user proper permissions
        //backup file if it already exists before overwrite
        if (File.Exists(filepath))
        {

            string backupFilepath = filepath + "_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".backup";
            try
            {
                File.Copy(filepath, backupFilepath, true);
                Debug.Log($"Backup created successfully at: {backupFilepath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create backup: {e.Message}");
                return; // Abort writing if backup fails (optional safety check)
            }
        }
        
        File.WriteAllText(filepath, saveJSON.ToString());
        //Debug.Log(saveJSON.ToString());
    }

    JObject GetMapMetadataJSON()
    {
        JObject metadata = new JObject(
            new JProperty("name", mapName),
            new JProperty("version", version += 1),
            new JProperty("saveVersion", _currentNewSaveVersion),
            new JProperty("date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new JProperty("author", mapAuthor)
            );
        
        return metadata;
    }

    JObject GetMapPlayersJSON()
    {
        JObject players = new JObject(
            new JProperty("red", 
                new JObject(new JProperty("position", 
                    new JObject(
                        new JProperty("x", redPlayerSpawn.x),
                        new JProperty("y", redPlayerSpawn.y),
                        new JProperty("z", redPlayerSpawn.z))
                ),
                new JProperty("rotation", 
                    new JObject(
                        new JProperty("w", redPlayerSpawnRotation.w),
                        new JProperty("x", redPlayerSpawnRotation.x),
                        new JProperty("y", redPlayerSpawnRotation.y),
                        new JProperty("z", redPlayerSpawnRotation.z))
                    )
                )),
            new JProperty("blue", new JObject(
                new JProperty("position", 
                    new JObject(
                        new JProperty("x", bluePlayerSpawn.x),
                        new JProperty("y", bluePlayerSpawn.y),
                        new JProperty("z", bluePlayerSpawn.z))
                ),
                new JProperty("rotation", 
                    new JObject(
                        new JProperty("w", bluePlayerSpawnRotation.w),
                        new JProperty("x", bluePlayerSpawnRotation.x),
                        new JProperty("y", bluePlayerSpawnRotation.y),
                        new JProperty("z", bluePlayerSpawnRotation.z))
                )
            )));
        
        return players;
        
        /*"players": {
            "red": {
                "position": {xyz},
                "rotation": {wxyz}
                }
            },
            "blue"...
        }*/
    }
    
    JObject GetMapGridBlocksJSON()
    {
        JObject blocks = new JObject();

        foreach (Block block in currentStaticBlocks.Values)
        {
            addGridBlockToJObject(blocks, block);
        }

        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            addGridBlockToJObject(blocks, complexBlock);
        }
        
        return blocks;
    }

    public void addGridBlockToJObject(JObject blockList, Placeable block)
    {
        PlaceableKey key = new PlaceableKey(block.listID, block.baseID, block.varientID, block.transform.rotation);
        string keyString = key.getKeyString();
        if (!blockList.ContainsKey(keyString))
        {
            blockList.Add(keyString, new JArray());
        }
        Vector3 pos = Map.WorldToHalfGrid(block.transform.position);
        ((JArray)blockList[keyString]).Add(new JArray{pos.x, pos.y, pos.z});
    }
    
    JObject GetMapDecorJSON()
    {
        JObject decor = new JObject();

        return decor;
    }
    
    JObject GetMapEntitiesJSON()
    {
        JObject entities = new JObject();

        return entities;
    }

    #region old text based save
    public void SaveMapToFileOLD(string filename)
    {
        if(redPlayerSpawn == null || redPlayerSpawnRotation == null || bluePlayerSpawn == null || bluePlayerSpawnRotation == null)
        {
            //TODO allow this but mark map as incomplete/unplayable
            Debug.Log("ERROR missing players from map, cannot save as is");
            return;
        }

        string mapJSON = "{\n";

        //METADATA
        mapJSON += "\"metadata\": " + GetMapMetadataOLD();

        //Players
        mapJSON += GetPlayersJSON_OLD();

        //Blocks Data
        mapJSON += GetBlocksListJSON_OLD();

        //end file
        mapJSON += "\n}";

        Debug.Log(mapJSON);
        //save file to text
        string filepath = Application.streamingAssetsPath + "/" + filename + ".json";

        //overwrites file if it already exists
        File.WriteAllText(filepath, mapJSON);
    }

    private string GetMapMetadataOLD()
    {
        return "{\n" +
               "\"version\": " + _currentNewSaveVersion +
        "\n},\n";
    }

    private string GetBlocksListJSON_OLD()
    {
        string blocksListJSON = "";

        //start of blocks array
        blocksListJSON += "\"blocks\": [\n";

        bool firstBlock = true;
        //fill blocks array
        //static
        foreach (Block block in currentStaticBlocks.Values)
        {
            //add comma after previous block
            if (!firstBlock) { blocksListJSON += ",\n"; }
            else { firstBlock = false; }
            blocksListJSON += ConvertPlaceableToJSON_OLD(block);
        }
        //complex
        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            //add comma after previous block
            if (!firstBlock) { blocksListJSON += ",\n"; }
            else { firstBlock = false; }
            blocksListJSON += ConvertPlaceableToJSON_OLD(complexBlock);
        }

        //finish blocks array
        blocksListJSON += "\n]";

        return blocksListJSON;
    }

    private string GetPlayersJSON_OLD()
    {
        return "\"players\": {\n" +
                    "\"red\": {\n" +
                        GetPositionJSON_OLD(redPlayerSpawn) +
                        GetRotationJSON_OLD(redPlayerSpawnRotation) +
                    "\n},\n"    +
                    "\"blue\": {\n" +
                        GetPositionJSON_OLD(bluePlayerSpawn) +
                        GetRotationJSON_OLD(bluePlayerSpawnRotation) +
              "}\n},\n";
    }

    private string ConvertPlaceableToJSON_OLD(Placeable placeable)
    {
        //start block
        string json = "{\n";

        //INFO
        json += "\"info\": {\n";

        //blockID
        json += "\"list_id\": \"" + placeable.listID + "\",\n";
        json += "\"base_id\": \"" + placeable.baseID + "\",\n";
        json += "\"varient_id\": \"" + placeable.varientID + "\",\n";

        //position
        json += GetPositionJSON_OLD(WorldToHalfGrid(placeable.transform.position));

        //rotation
        json += GetRotationJSON_OLD(placeable.transform.rotation); //add comma to this function if adding more parts to block info

        //finish info
        json += "}\n";

        //finish block
        json += "}\n";

        return json;
    }

    private string GetPositionJSON_OLD(Vector3 pos)
    {
        return "\"position\": {\n" +
            "\"x\": " + pos.x + ",\n" +
            "\"y\": " + pos.y + ",\n" +
            "\"z\": " + pos.z + "\n" +
            "},\n";
    }

    private string GetRotationJSON_OLD(Quaternion rot)
    {
        return "\"rotation\": {\n" +
            "\"w\": " + rot.w + ",\n" +
            "\"x\": " + rot.x + ",\n" +
            "\"y\": " + rot.y + ",\n" +
            "\"z\": " + rot.z + "\n" +
            "}\n";//add comma here if more fields added to block ie, components
    }
    
    #endregion

    //todo make this more useful maybe
    public void DebugMap()
    {
        int odd = 0;
        int even = 0;
        foreach (Vector3Int position in currentStaticBlocks.Keys)
        {

            if (position.x % 2 != 0 || position.y % 2 != 0 || position.z % 2 != 0)
            {
                odd++;
            }
            else
            {
                even++;
            }
        }
        Debug.Log("odd: "  + odd + ", even: " + even);
    }
}
