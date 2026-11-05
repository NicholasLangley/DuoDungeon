using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelector : MonoBehaviour
{
    [SerializeField]
    GameObject blockButtonPrefab;
    [SerializeField]
    GameObject blockButtonGrid;
    [SerializeField]
    ObjectPlacer blockPlacer;

    public void populateButtons(BlockMasterList blockMasterList)
    {
        if (blockMasterList.blocksDictionary == null) { blockMasterList.GenerateDict(); }
        foreach (BlockVarientList varientList in blockMasterList.blocksDictionary.Values)
        {
            GameObject newButton = Instantiate(blockButtonPrefab, blockButtonGrid.transform);
            BlockSelectionButton blockButton = newButton.GetComponent<BlockSelectionButton>();
            BlockData blockData = varientList.GetVarient("blank");

            string baseID;
            string varientID;
            Placeable placeable = blockData;
            baseID = placeable.baseID;
            varientID = placeable.varientID;
            /*ComplexBlock complexBlock = objectGameObject.GetComponent<ComplexBlock>();
            if(complexBlock != null)
            {
                baseID = complexBlock.baseID;
                varientID = complexBlock.varientID;
            }
            else
            {
                Block block = objectGameObject.GetComponent<Placeable>();
                baseID = block.baseID;
                varientID = block.varientID;
            }*/
            
            blockButton.SetBlock(placeable, varientList.baseID);
            newButton.GetComponent<Button>().onClick.AddListener(() => selectBlock(blockButton.placeable, blockMasterList.listID, baseID, varientID));
        }
    }

    public void selectBlock(Placeable placeable, string listID, string baseID, string varientID)
    {
        blockPlacer.SetBlock(placeable, listID, baseID, varientID);
    }


}
