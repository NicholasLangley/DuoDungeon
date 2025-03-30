using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


public class BlockBuilder
{
    UltimateList ultimateList;

    public BlockBuilder(UltimateList lists)
    {
        ultimateList = lists;
    }

    public enum BlockType { Basic, Door, Ice }

    public GameObject BuildBlock(BlockType type, JToken blockEntry)
    {
        GameObject newBlock = GeneratePrefab((string)blockEntry["list_id"], (string)blockEntry["base_id"], (string)blockEntry["varient_id"]);

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



    GameObject GeneratePrefab(string listID, string baseID, string varientID)
    {
        BlockMasterList masterList = ultimateList.GetMasterList(listID);
        GameObject prefab = masterList.GetBlock(baseID, varientID);
        prefab.GetComponent<IPlaceable>().listID = listID;
        return prefab;
    }

    public void setBlockPosition(GameObject block, JToken position)
    {
        Vector3 blockPos = parseJSONPosition(position);
        block.transform.position = blockPos;
    }

    public static Vector3 parseJSONPosition(JToken pos)
    {
        return new Vector3((int)pos["x"], (int)pos["y"], (int)pos["z"]);
    }

    public void setBlockRotation(GameObject block, JToken rotation)
    {
        Quaternion blockRotation = parseJSONRotation(rotation);
        block.transform.rotation = blockRotation;
    }

    public static Quaternion parseJSONRotation(JToken rotation)
    {
        return new Quaternion((float)rotation["x"], (float)rotation["y"], (float)rotation["z"], (float)rotation["w"]);
    }
}
