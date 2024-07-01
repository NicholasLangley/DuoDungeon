using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BlockBuilder
{
    GameObject[] allPrefabs;

    public enum BlockType { Basic, Door, Ice }

    public BlockBuilder()
    {

    }

    public void loadAllPrefabs()
    {
        allPrefabs = Resources.LoadAll<GameObject>("Blocks");
    }

    public void clearAllPrefabs()
    {
        allPrefabs = null;
    }

    public GameObject BuildBlock(BlockType type, Newtonsoft.Json.Linq.JToken blockInfo)
    {
        switch (type)
            {
            case BlockType.Door:
                return null;

            case BlockType.Ice:
                return null;

            default:
                return createDefaultBlock(blockInfo);

        }    
    }


    GameObject createDefaultBlock(Newtonsoft.Json.Linq.JToken blockInfo)
    {
        int blockIndex = (int)blockInfo["block_id"];
        GameObject newBlock = GameObject.Instantiate(allPrefabs[blockIndex]);
        Vector3 blockPos = new Vector3((int)blockInfo["x"], (int)blockInfo["y"], (int)blockInfo["z"]);
        newBlock.transform.position = blockPos;

        return newBlock;
    }
}
