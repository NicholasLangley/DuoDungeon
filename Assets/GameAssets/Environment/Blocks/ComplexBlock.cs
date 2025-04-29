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
public class PartialComplexBlock
{
    public Vector3Int gridPosition;
    public Block block;
}

//a unit contains the 2 block parts that a single component block is split into
public class ComplexBlockUnit
{
    //The larger block, contains the midpoint
    public PartialComplexBlock mainBlock;

    //sticks up or down into the next grid space from the main block
    public PartialComplexBlock subBlock;
}
//Blocks that may move / occupy multiple grid spaces at once. Will calculate sub blocks that conform to the grid
public class ComplexBlock : MonoBehaviour, IPlaceable
{
    public string listID { get; set; }
    [field: SerializeField] public string baseID { get; set; }
    [field: SerializeField] public string varientID { get; set; }

    GameObject gridSplitBlocksParent;

    List<ComplexBlockUnit> gridBlockUnits;

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

        gridBlockUnits = new List<ComplexBlockUnit>();

        foreach (ComponentBlockDefinition componentBlock in componentBlocks)
        {
            ComplexBlockUnit unit = new ComplexBlockUnit();

            PartialComplexBlock partialBlock = new PartialComplexBlock();
            partialBlock.gridPosition = Map.GetIntVector3(transform.position +  transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z));

            Block newBlock = gridSplitBlocksParent.AddComponent<Block>();
            newBlock.blockSides = componentBlock.sides;
            partialBlock.block = newBlock;

            unit.mainBlock = partialBlock;

            gridBlockUnits.Add(unit);
        }
    }

    public List<ComplexBlockUnit> GetGridBlockUnits()
    {
        return gridBlockUnits;
    }


}
