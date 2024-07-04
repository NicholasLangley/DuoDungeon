using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelector : MonoBehaviour
{
    [SerializeField]
    BlockList blockList;

    [SerializeField]
    GameObject blockButtonPrefab;
    [SerializeField]
    GameObject blockButtonGrid;
    [SerializeField]
    BlockPlacer blockPlacer;

    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < blockList.GetLength(); i++)
        {
            GameObject newButton = Instantiate(blockButtonPrefab, blockButtonGrid.transform);
            BlockSelectionButton blockButton = newButton.GetComponent<BlockSelectionButton>();
            blockButton.SetBlock(i, blockList.getBlock(i));

            newButton.GetComponent<Button>().onClick.AddListener(() => selectBlock(blockButton.block));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectBlock(Block block)
    {
        Debug.Log("Selected Block " + block.name);
        blockPlacer.setBlock(block);
    }
}
