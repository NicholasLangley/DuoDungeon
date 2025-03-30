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
            GameObject blockGameObject = varientList.GetBlock("blank");
            Block block = blockGameObject.GetComponent<Block>();
            blockButton.SetBlock(blockGameObject, varientList.baseID);
            newButton.GetComponent<Button>().onClick.AddListener(() => selectBlock(blockButton.block, blockMasterList.listID, block.baseID, block.varientID));
        }
    }

    public void selectBlock(GameObject block, string listID, string baseID, string varientID)
    {
        blockPlacer.SetBlock(block, listID, baseID, varientID);
    }


}
