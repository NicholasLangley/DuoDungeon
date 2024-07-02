using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BlockBuilder
{
    BlockPrefabGenerator blockPrefabGenerator;

    public enum BlockType { Basic, Door, Ice }

    public BlockBuilder(BlockPrefabGenerator bfg)
    {
        blockPrefabGenerator = bfg;
    }


    public Block BuildBlock(BlockType type, JToken blockEntry)
    {
        Block newBlock = blockPrefabGenerator.generatePrefab((int)blockEntry["block_id"]);

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





    public void setBlockPosition(Block block, JToken position)
    {
        Vector3 blockPos = new Vector3((int)position["x"], (int)position["y"], (int)position["z"]);
        block.transform.position = blockPos;
    }

    public void setBlockRotation(Block block, JToken rotation)
    {
        //Quaternion blockRotation = new Quaternion((int)rotation["x"], (int)rotation["y"], (int)rotation["z"], 0);
        //block.transform.rotation = blockRotation;
    }
}
