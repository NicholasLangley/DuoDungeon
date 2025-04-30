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

    float _offsetThreshold = 0.01f;

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

        float distanceOffsetFromGrid = 0.0f;
        //used to find side of block affected, offset will be in the opposite direction of this
        Vector3 gravityDir = Vector3.down;
        //only FGMs can be offset from the grid, we use the gravity direction to get any possible offset
        FullGridMoveable fgm = gameObject.GetComponent<FullGridMoveable>();
        if(fgm != null)
        {
            gravityDir = fgm.gravityDirection;
            Vector3 offsetVector = new Vector3(-gravityDir.x * transform.position.x, gravityDir.y * transform.position.y, gravityDir.z * transform.position.z);
            distanceOffsetFromGrid = Vector3.Distance(offsetVector, Vector3.zero);
            distanceOffsetFromGrid = distanceOffsetFromGrid - Mathf.Floor(distanceOffsetFromGrid);
        }

        //no offset, just need to copy the component block as is
        if (distanceOffsetFromGrid <= _offsetThreshold)
        {
            foreach (ComponentBlockDefinition componentBlock in componentBlocks)
            {
                GridSplitBlock block = new GridSplitBlock();
                block.gridPosition = Map.GetGridSpace(transform.position + transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z));

                Block newBlock = gridSplitBlocksParent.AddComponent<Block>();
                newBlock.blockSides = new BlockSideDefinitions();
                newBlock.blockSides.CopyBlockSides(componentBlock.sides);
                block.block = newBlock;

                gridBlocks.Add(block);
            }
            return;
        }

        //offset detected, need to potentially create 2 blocks, and adjust them based on the offset
        foreach (ComponentBlockDefinition componentBlock in componentBlocks)
        {
            //The block that is still in the initial unoffset block grid
            GridSplitBlock initialGridBlock = new GridSplitBlock();
            initialGridBlock.gridPosition = Map.GetGridSpace(transform.position + transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z));

            Block newBlock = gridSplitBlocksParent.AddComponent<Block>();
            newBlock.blockSides = new BlockSideDefinitions();
            newBlock.blockSides.CopyBlockSides(componentBlock.sides);
            initialGridBlock.block = newBlock;

            BlockSide topOffsetSide = newBlock.GetOrientedTopSide(gravityDir);
            topOffsetSide.AddHeightOffset(distanceOffsetFromGrid);
     
            BlockSide bottomOffsetSide = newBlock.GetOrientedTopSide(-gravityDir);
            bottomOffsetSide.AddHeightOffset(-distanceOffsetFromGrid);

            gridBlocks.Add(initialGridBlock);

            if(topOffsetSide.CheckIfSideExtendsToNextGrid())
            {
                //the block that forms due to the offset pushing the component block into a new grid
                GridSplitBlock offsetGridBlock = new GridSplitBlock();
                offsetGridBlock.gridPosition = Map.GetGridSpace(transform.position + (transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z)) - gravityDir);
                Block newOffsetBlock = gridSplitBlocksParent.AddComponent<Block>();
                newOffsetBlock.blockSides = new BlockSideDefinitions();
                newOffsetBlock.blockSides.CopyBlockSides(componentBlock.sides);
                offsetGridBlock.block = newOffsetBlock;

                
                topOffsetSide = newOffsetBlock.GetOrientedTopSide(gravityDir);
                topOffsetSide.AddHeightOffset(distanceOffsetFromGrid - 1.0f);

                bottomOffsetSide = newOffsetBlock.GetOrientedTopSide(-gravityDir);
                bottomOffsetSide.AddHeightOffset(-distanceOffsetFromGrid + 1.0f);

                gridBlocks.Add(offsetGridBlock);
            }
        }
    }

    public List<GridSplitBlock> GetGridSplitBlocks()
    {
        return gridBlocks;
    }


}
