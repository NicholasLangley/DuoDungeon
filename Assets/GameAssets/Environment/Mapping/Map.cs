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
    //TODO maybe only load this temporarily while loading the maps? only if performance really tanks eventually from having poone of every block loaded, even unused ones
    UltimateList ultimateList;
    
    //todo can these be combined? idk since complex have more logic
    Dictionary<Vector3Int, BlockInstance> currentStaticBlocks;
    //List<ComplexBlock> currentComplexBlocks;
    private GameObject _rendererParent;
    private Dictionary<string, FlyweightRenderer> _renderers;
    
    Dictionary<Vector3Int, Wall> currentWalls;

    public Vector3 redPlayerSpawn, bluePlayerSpawn;
    public Quaternion redPlayerSpawnRotation, bluePlayerSpawnRotation;

    //metadata
    public static string _currentNewSaveVersion = "0.25";
    //TODO set these in level editor, maybe keep list of previous editors?
    string mapAuthor = "Nicole Langley" ;
    public string mapName { get; set; }
    public int version { get; set; }
    

    public Map(UltimateList ultimateList)
    {
        currentStaticBlocks = new Dictionary<Vector3Int, BlockInstance>();
        _rendererParent = new  GameObject("FlyweightRenderers");
        _renderers = new Dictionary<string, FlyweightRenderer>();
        //currentComplexBlocks = new List<ComplexBlock>();
        
        version = 0;

        this.ultimateList = ultimateList;
    }

    public void ClearMap()
    {

        currentStaticBlocks.Clear();
        ResetRenderers();

        /*foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            GameObject.Destroy(complexBlock.gameObject);
        }
        currentComplexBlocks.Clear();*/
    }


    #region static blocks
    public void AddBlockToWorldPos(Vector3 worldPos,  BlockInstance block)
    {
        Vector3Int halfGridPosition = WorldToHalfGrid(worldPos);
        AddBlockToHalfGridPos(halfGridPosition, block);
    }

    public void AddBlockToHalfGridPos(Vector3Int halfGridPos, BlockInstance block)
    {
        //overwrite
        if (currentStaticBlocks.ContainsKey(halfGridPos)) 
        {
            RemoveBlockFromHalfGrid(halfGridPos);
        }
        currentStaticBlocks.Add(halfGridPos, block);
        
        //get / create renderer for mesh
        AddBlockToRenderer( HalfGridToWorld(halfGridPos), block);
        

        //todo separate complex logic???
    }

    public void AddBlockToRenderer(Vector3 worldPos, BlockInstance block)
    {
        FlyweightRenderer renderer;
        if (_renderers.ContainsKey(block.blockData.GetIdString()))
        {
            renderer = _renderers[block.blockData.GetIdString()];
        }
        else
        {
            GameObject newRenderer = new GameObject("Renderer: " + block.blockData.GetIdString());
            newRenderer.transform.SetParent(_rendererParent.transform);
            renderer = newRenderer.AddComponent<FlyweightRenderer>();
            renderer.SetMesh(block.blockData.mesh);
            renderer.SetMaterial(block.blockData.material);
            _renderers.Add(block.blockData.GetIdString(), renderer);
        }
        block.SetRenderIndex(renderer.GetNextIndex());
        renderer.AddTransform(new FlyweightTransform(worldPos, block.rotation));
    }

    private void RemoveBlockFromWorld(Vector3 worldPos)
    {
        Vector3Int halfGridPosition = WorldToHalfGrid(worldPos);
        RemoveBlockFromHalfGrid(halfGridPosition);
    }
    
    private void RemoveBlockFromHalfGrid(Vector3Int halfGridPos)
    {
        if (currentStaticBlocks.ContainsKey(halfGridPos))
        {
            //disable renderer index (note since block is removed entirely from map this is permanent)
            //render index is orphaned
            BlockInstance block = currentStaticBlocks[halfGridPos];
            DisableBlockInRenderer(block.blockData.GetIdString(), block.rendererIndex);
            
            currentStaticBlocks.Remove(halfGridPos);
        }
        //todo seperate complex logic????
    }

    public void DisableBlockInRenderer(string idString, int index)
    {
        if (_renderers.ContainsKey(idString))
        {
            _renderers[idString].DisableTransform(index);
        }
    }

    private BlockInstance? GetStaticBlock(Vector3Int halfGridPosition)
    {
        if (currentStaticBlocks.ContainsKey(halfGridPosition)) { return currentStaticBlocks[halfGridPosition]; }

        return null;
    }

    private BlockInstance? GetStaticBlock(Vector3 worldPosition)
    {
        Vector3Int halfGridPosition = WorldToHalfGrid(worldPosition);
        return (GetStaticBlock(Map.GetIntVector3(halfGridPosition)));
    }
    #endregion

    #region complexBlocks

    public List<ComplexBlock> GetComplexBlocksList()
    {
        return null;//currentComplexBlocks;
    }

    public void AddComplexBlock(ComplexBlock complexBlock)
    {
        //currentComplexBlocks.Add(complexBlock);
    }

    //todo fix complex blocks
    /*
    private BlockOLD GetComplexBlockPart(Vector3 position, GameObject requestor)
    {
        return GetComplexBlockPart(Map.GetIntVector3(position), requestor);
    }

    private BlockOLD GetComplexBlockPart(Vector3Int position, GameObject requestor)
    {
        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            if (complexBlock.gameObject != requestor)
            {
                foreach (BlockOLD gridBlock in complexBlock.GetGridBlocks())
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
        BlockOLD blockOldPart = GetComplexBlockPart(position, null);
        if (blockOldPart == null) { return; }
        ComplexBlock  complex = blockOldPart.GetComponentInParent<ComplexBlock>();
        currentComplexBlocks.Remove(complex);
        GameObject.Destroy(complex.gameObject);
    }*/

    #endregion

    public void RemoveBlockAtLocation(Vector3 pos)
    {
        Vector3 position = WorldToHalfGrid(pos);
        BlockInstance? block = GetBlockAtGridPosition(position, null, Vector3.down);
        if (block == null){ return; }
        /*
         TODO reintegrate
        if (block.GetComponentInParent<ComplexBlock>() != null)
        {
            RemoveComplexBlockAtLocation(position);
        }
        else*/
        RemoveBlockFromWorld(Map.GetIntVector3(position));
    }

    public BlockInstance? GetBlockAtGridPosition(Vector3 worldPos, GameObject requestor, Vector3 downVector)
    {
        return GetBlockAtGridPosition(Map.GetIntVector3(worldPos), requestor, downVector);
    }

    public BlockInstance? GetBlockAtGridPosition(Vector3Int worldPos, GameObject requestor, Vector3 downVector)
    {
        Vector3Int halfGridPos = WorldToHalfGrid(worldPos);
        BlockInstance? staticBlock = GetStaticBlock(halfGridPos);
        // todo fix
        //BlockOLD complexBlockOldPart = GetComplexBlockPart(halfGridPos, requestor);
        
        /*
         TODO reintegrate complex blocks
        if (staticBlock == null) { return complexBlockOldPart; }
        if (complexBlockOldPart == null) { return staticBlockOld; }

        //need to find highest block relative to requestor and return that
        if (staticBlockOld.GetOrientedTopSide(downVector).centerHeight > complexBlockOldPart.GetOrientedTopSide(downVector).centerHeight) { return staticBlockOld; }
        return complexBlockOldPart;
        */
        return staticBlock;
    }

    public BlockInstance? GetCurrentlyOccupiedBlock(GameObject requestor, DownDirection downDir)
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
        //TODO fix complex
        //BlockOLD complexBlockOld = GetComplexBlockPart(gridPosition, requestor);
        //if (complexBlockOld != null ) { return complexBlockOld; }
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

    #region saving / loading
    
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

        foreach (Vector3Int halfGridPos in currentStaticBlocks.Keys)
        {
            addGridBlockToJObject(blocks, halfGridPos, currentStaticBlocks[halfGridPos]);
        }

        /*
         TODO fix complex
        foreach (ComplexBlock complexBlock in currentComplexBlocks)
        {
            addGridBlockToJObject(blocks, complexBlock);
        }
        */
        
        return blocks;
    }

    public void addGridBlockToJObject(JObject blockList, Vector3Int halfGridPos, BlockInstance block)
    {
        PlaceableKey key = new PlaceableKey(block.blockData.listID, block.blockData.baseID, block.blockData.varientID, block.rotation);
        string keyString = key.getKeyString();
        if (!blockList.ContainsKey(keyString))
        {
            blockList.Add(keyString, new JArray());
        }
        ((JArray)blockList[keyString]).Add(new JArray{halfGridPos.x, halfGridPos.y, halfGridPos.z});
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
    
    public BlockData LoadBlockData (string listID, string baseID, string varientID)
    {
        //TODO if needed need to laod / unload the full list. then only keep the necessary flyweights around
        // should be a minor performance improvement if the list becomes too massive
        BlockData data = ultimateList.GetMasterList(listID).GetBlock(baseID, varientID);

        return data;
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

    public void ResetRenderers()
    {
        foreach (FlyweightRenderer renderer in _renderers.Values)
        {
            GameObject.Destroy(renderer.gameObject);
        }
        _renderers.Clear();
    }
}
