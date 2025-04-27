using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentBlockDefinition
{
    [SerializeField]
    public Vector3Int offsetFromCenter;
    [SerializeField]
    public BlockSideDefinitions sides;
}

public class GridSplitBlock
{
    public Vector3Int gridPosition;
    public Block block;
}

//Blocks that may move / occupy multiple grid spaces at once. Will calculate sub blocks that conform to the grid
public class ComplexBlock : MonoBehaviour, IPlaceable
{
    public string listID { get; set; }
    [field: SerializeField] public string baseID { get; set; }
    [field: SerializeField] public string varientID { get; set; }

    GameObject gridSplitBlocksParent;

    List<GridSplitBlock> gridBlocks;

    [SerializeField]
    List<ComponentBlockDefinition> componentBlocks;

    // Start is called before the first frame update
    void OnEnable()
    {
        CalculateGridBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateGridBlocks()
    {
        Destroy(gridSplitBlocksParent);
        gridSplitBlocksParent = new GameObject("gridSplitBlocksParent");
        gridSplitBlocksParent.transform.parent = this.transform;

        gridBlocks = new List<GridSplitBlock>();
        foreach (ComponentBlockDefinition componentBlock in componentBlocks)
        {
            GridSplitBlock gridBlock = new GridSplitBlock();
            gridBlock.gridPosition = Map.GetIntVector3(transform.position +  transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z));

            Block newBlock = gridSplitBlocksParent.AddComponent<Block>();
            newBlock.blockSides = componentBlock.sides;
            gridBlock.block = newBlock;

            gridBlocks.Add(gridBlock);
        }
    }

    public List<GridSplitBlock> GetGridBlocks()
    {
        return gridBlocks;
    }


}
