using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockVarientListScriptableObject", menuName = "ScriptableObjects/BlockVarientList")]
public class BlockVarientList : ScriptableObject
{
    [SerializeField]
    public string baseID;
    [SerializeField]
    List<GameObject> blockVarients;

    Dictionary<string, GameObject> varientDictionary;

    public void generateDict()
    {
        varientDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject blockObj in blockVarients)
        {
            Block b = blockObj.GetComponent<Block>();
            varientDictionary.Add(b.varientID, blockObj);
        }
    }

    public GameObject GetBlock(string varientID)
    {
        return varientDictionary[varientID];
    }

    public int GetLength()
    {
        return blockVarients.Count;
    }
}
