using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
