using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The raw block definitions of the complex block. If the transform is aligned with the grid, this is what the final blocks will be
[System.Serializable]
public class ComponentBlockDefinition
{
    [SerializeField]
    public Vector3Int offsetFromCenter;
    [SerializeField]
    public BlockSideDefinitions sides;
}

//a partial block that was split to be alligned with the grid
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
            GridSplitBlock block1 = new GridSplitBlock();
            block1.gridPosition = Map.GetIntVector3(transform.position +  transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z));

            Block newBlock = gridSplitBlocksParent.AddComponent<Block>();
            newBlock.blockSides = componentBlock.sides;
            block1.block = newBlock;

            gridBlocks.Add(block1);
        }
    }

    public List<GridSplitBlock> GetGridSplitBlocks()
    {
        return gridBlocks;
    }


}
