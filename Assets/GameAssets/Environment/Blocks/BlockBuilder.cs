using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


//deprecated probably
/*
public class BlockBuilder
{
    UltimateList ultimateList;

    public BlockBuilder(UltimateList lists)
    {
        ultimateList = lists;
    }

    //todo probably have to abandon for flyweight
    public GameObject BuildBlock(PlaceableKey key, float x, float y, float z)
    {
        GameObject newBlock = GeneratePrefab(key.listID, key.baseID , key.varientID);

        newBlock.transform.position = Map.HalfGridToWorld(new Vector3(x,y,z));
        newBlock.transform.rotation = new Quaternion(key.rotationX, key.rotationY, key.rotationZ, key.rotationW);

        return newBlock;
    }
    
    //todo deprecate fully and remove with load version 0.1
    public GameObject BuildBlockOLD(JToken blockEntry)
    {
        GameObject newBlock = GeneratePrefab((string)blockEntry["list_id"], (string)blockEntry["base_id"], (string)blockEntry["varient_id"]);

        setBlockPositionOLD(newBlock, blockEntry["position"]);
        setBlockRotationOLD(newBlock, blockEntry["rotation"]);

        return newBlock;
    }



    GameObject GeneratePrefab(string listID, string baseID, string varientID)
    {
        BlockMasterList masterList = ultimateList.GetMasterList(listID);
        GameObject prefab = masterList.GetBlock(baseID, varientID);
        prefab.GetComponent<PlaceableMonoBehaviour>().listID = listID;
        return prefab;
    }

    //todo deprecate with load 0.1
    public void setBlockPositionOLD(GameObject block, JToken position)
    {
        Vector3 blockPos = Map.HalfGridToWorld(parseJSONPosition(position));
        block.transform.position = blockPos;
    }

    public static Vector3 parseJSONPosition(JToken pos)
    {
        return new Vector3((int)pos["x"], (int)pos["y"], (int)pos["z"]);
    }

    //todo deprecate with load 0.1
    public void setBlockRotationOLD(GameObject block, JToken rotation)
    {
        Quaternion blockRotation = parseJSONRotation(rotation);
        block.transform.rotation = blockRotation;
    }

    public static Quaternion parseJSONRotation(JToken rotation)
    {
        return new Quaternion((float)rotation["x"], (float)rotation["y"], (float)rotation["z"], (float)rotation["w"]);
    }
}
*/