using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class BlockBuilder
{
    BlockList blockList;

    public BlockBuilder(BlockList list)
    {
        blockList = list;
    }

    public enum BlockType { Basic, Door, Ice }

    public Block BuildBlock(BlockType type, JToken blockEntry)
    {
        Block newBlock = generatePrefab((int)blockEntry["block_id"]);

        setBlockPosition(newBlock, blockEntry["position"]);
        setBlockRotation(newBlock, blockEntry["rotation"]);

        switch (type)
            {
            case BlockType.Door:
                break;

            case BlockType.Ice:
                break;

            default:
                break;
        }

        return newBlock;
    }



    Block generatePrefab(int blockID)
    {
        Block prefab = GameObject.Instantiate(blockList.getBlock(blockID));
        prefab.blockID = blockID;
        return prefab;
    }

    public void setBlockPosition(Block block, JToken position)
    {
        Vector3 blockPos = new Vector3((int)position["x"], (int)position["y"], (int)position["z"]);
        block.transform.position = blockPos;
    }

    public void setBlockRotation(Block block, JToken rotation)
    {
        Quaternion blockRotation = new Quaternion((float)rotation["x"], (float)rotation["y"], (float)rotation["z"], (float)rotation["w"]);
        block.transform.rotation = blockRotation;
    }
}
