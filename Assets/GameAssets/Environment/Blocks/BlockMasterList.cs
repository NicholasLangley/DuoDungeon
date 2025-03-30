using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockMasterListScriptableObject", menuName = "ScriptableObjects/BlockMasterList")]
public class BlockMasterList : ScriptableObject
{
    [SerializeField]
    public string listID;
    [SerializeField]
    List<BlockVarientList> blockVarientLists;

    public Dictionary<string, BlockVarientList> blocksDictionary;

    public void GenerateDict()
    {
        blocksDictionary = new Dictionary<string, BlockVarientList>();
        foreach (BlockVarientList list in blockVarientLists)
        {
            list.generateDict();
            blocksDictionary.Add(list.baseID, list);
        }
    }

    public GameObject GetBlock(string baseID, string varientID)
    {
        if (blocksDictionary == null) { GenerateDict(); }
        return blocksDictionary[baseID].GetBlock(varientID);
    }

    public int GetLength()
    {
        return blockVarientLists.Count;
    }
}
