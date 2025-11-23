using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BlockVarientListScriptableObject", menuName = "ScriptableObjects/BlockVarientList")]
public class BlockVarientList : ScriptableObject
{
    //The common properties and scripts shared by all varients
    //examples include: The Pushable script for pushables, the block script (block side heights and properties, etc.)
    //This way any changes just have to be made once for all varients
    [SerializeField]
    [FormerlySerializedAs("BlockPropertiesAndScriptsObject")]
    GameObject CommonPropertiesAndScriptsObject;

    [SerializeField]
    public string baseID;
    [SerializeField]
    [FormerlySerializedAs("blockVarients")]
    List<GameObject> varients;
    

    Dictionary<string, GameObject> varientDictionary;

    public void generateDict()
    {
        varientDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject varientObj in varients)
        {
            VarientID id = varientObj.GetComponent<VarientID>();
            varientDictionary.Add(id.varientID, varientObj);
        }
    }

    public GameObject GetBlock(string varientID)
    {
        GameObject baseBlock = Instantiate(CommonPropertiesAndScriptsObject);

        Block blockComponent = baseBlock.GetComponent<Block>();
        if (blockComponent != null) { blockComponent.varientID = varientID; }

        Instantiate(varientDictionary[varientID], baseBlock.transform);
        
        return baseBlock;
    }

    public int GetLength()
    {
        return varients.Count;
    }
}
