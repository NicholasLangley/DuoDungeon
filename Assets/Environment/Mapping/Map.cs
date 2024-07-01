using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    GameObject mapParent;

    Dictionary<Vector3Int, GameObject> currentBlocks;

    public Map()
    {
        currentBlocks = new Dictionary<Vector3Int, GameObject>();

        mapParent = new GameObject();
        mapParent.name = "MAP";
    }

    public void ClearMap()
    {
        foreach (GameObject block in currentBlocks.Values)
        {
            GameObject.Destroy(block);
        }
        currentBlocks.Clear();
    }

    public void AddBlock(Vector3Int position, GameObject block)
    {
        currentBlocks.Add(position, block);
        block.transform.parent = mapParent.transform;
    }


}
