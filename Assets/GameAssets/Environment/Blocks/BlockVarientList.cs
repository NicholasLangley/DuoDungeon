using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BlockVarientListScriptableObject", menuName = "ScriptableObjects/BlockVarientList")]
public class BlockVarientList : ScriptableObject
{
    //deprecated
    //The common properties and scripts shared by all varients
    //examples include: The Pushable script for pushables, the block script (block side heights and properties, etc.)
    //This way any changes just have to be made once for all varients
    /*[SerializeField]
    [FormerlySerializedAs("BlockPropertiesAndScriptsObject")]
    GameObject CommonPropertiesAndScriptsObject;*/

    [SerializeField]
    public string baseID;

    [SerializeField] private List<BlockData> varientBlockDataObjects;
    

    Dictionary<string, BlockData> varientDictionary;

    public void generateDict()
    {
        varientDictionary = new Dictionary<string, BlockData>();
        foreach (BlockData varientData in varientBlockDataObjects)
        {
            varientDictionary.Add(varientData.varientID, varientData);
        }
    }

    public BlockData GetVarient(string varientID)
    {
        return varientDictionary[varientID];
    }

    public int GetLength()
    {
        return varientDictionary.Count;
    }
}
