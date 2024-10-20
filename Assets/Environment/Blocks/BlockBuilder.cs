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
        Vector3 blockPos = parseJSONPosition(position);
        block.transform.position = blockPos;
    }

    public static Vector3 parseJSONPosition(JToken pos)
    {
        return new Vector3((int)pos["x"], (int)pos["y"], (int)pos["z"]);
    }

    public void setBlockRotation(Block block, JToken rotation)
    {
        Quaternion blockRotation = parseJSONRotation(rotation);
        block.transform.rotation = blockRotation;
    }

    public static Quaternion parseJSONRotation(JToken rotation)
    {
        return new Quaternion((float)rotation["x"], (float)rotation["y"], (float)rotation["z"], (float)rotation["w"]);
    }
}
