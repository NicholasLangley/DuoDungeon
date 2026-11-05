using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ultimate list is a list of all masterlists, ie normal blocks vs complex blocks vs entities, etc.
/// masterlists are each a list of base blocks varient lists whithin a block type, ie full cubes vs half cubes
/// variant lists are all the possible varients of a base block, ie a red brick full cube vs a marble full cube
/// </summary>
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
