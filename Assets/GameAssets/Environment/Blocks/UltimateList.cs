using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UltimateListScriptableObject", menuName = "ScriptableObjects/UltimateList")]
public class UltimateList : ScriptableObject
{
    [SerializeField]
    public List<BlockMasterList> ultimateList;

    public BlockMasterList GetMasterList(string id)
    {
        foreach (BlockMasterList list in ultimateList)
        {
            if (string.Compare(list.listID, id) == 0) { return list; }
        }
        throw new ApplicationException("MasterBlockList not found: " + id);
    }
}
