using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BlockBuilder
{
    public enum BlockType { Basic, Door, Ice }

    public BlockBuilder()
    {

    }

    public GameObject BuildBlock(BlockType type, Newtonsoft.Json.Linq.JToken blockInfo)
    {
        GameObject newBlock = new GameObject();
        switch (type)
            {
            case BlockType.Door:
                break;

            case BlockType.Ice:
                break;

            default:
                BaseBlock block = newBlock.AddComponent<BaseBlock>();
                block.Initialize((bool)blockInfo["blocksMovement"], (bool)blockInfo["blocksProjectiles"]);
                break;

        }
        Vector3 blockPos = new Vector3((int)blockInfo["x"], (int)blockInfo["y"], (int)blockInfo["z"]);
        newBlock.transform.position = blockPos;

        return newBlock;
        
    }
}
