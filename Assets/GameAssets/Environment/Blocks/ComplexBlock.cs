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

//Blocks that may move / occupy multiple grid spaces at once. Will calculate sub blocks that conform to the grid
public class ComplexBlock : Placeable
{
    GameObject gridSplitBlocksParent;

    List<Block> gridBlocks;

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
        gridSplitBlocksParent.transform.localPosition = Vector3.zero;
        gridSplitBlocksParent.transform.localRotation = Quaternion.identity;

        gridBlocks = new List<Block>();

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
                Block singleBlock = gridSplitBlocksParent.AddComponent<Block>();
                singleBlock.blockSides = new BlockSideDefinitions();
                singleBlock.blockSides.CopyBlockSides(componentBlock.sides);
                singleBlock.SetGridPosition(Map.GetGridSpace(transform.position + transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z)));

                gridBlocks.Add(singleBlock);
            }
            return;
        }

        //offset detected, need to potentially create 2 blocks, and adjust them based on the offset
        foreach (ComponentBlockDefinition componentBlock in componentBlocks)
        {
            //The block that is still in the initial unoffset block grid
            Block transformAlignedBlock = gridSplitBlocksParent.AddComponent<Block>();
            transformAlignedBlock.blockSides = new BlockSideDefinitions();
            transformAlignedBlock.blockSides.CopyBlockSides(componentBlock.sides);
            transformAlignedBlock.SetGridPosition(Map.GetGridSpace(transform.position + transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z)));

            BlockSide topOffsetSide = transformAlignedBlock.GetOrientedTopSide(gravityDir);
            topOffsetSide.AddHeightOffset(distanceOffsetFromGrid);
     
            BlockSide bottomOffsetSide = transformAlignedBlock.GetOrientedTopSide(-gravityDir);
            bottomOffsetSide.AddHeightOffset(-distanceOffsetFromGrid);

            gridBlocks.Add(transformAlignedBlock);

            if(topOffsetSide.CheckIfSideExtendsToNextGrid())
            {
                //the block that forms due to the offset pushing the component block into a new grid
                Block OffsetBlock = gridSplitBlocksParent.AddComponent<Block>();
                OffsetBlock.blockSides = new BlockSideDefinitions();
                OffsetBlock.blockSides.CopyBlockSides(componentBlock.sides);
                OffsetBlock.SetGridPosition(Map.GetGridSpace(transform.position + (transform.rotation * new Vector3(componentBlock.offsetFromCenter.x, componentBlock.offsetFromCenter.y, componentBlock.offsetFromCenter.z)) - gravityDir));
                
                topOffsetSide = OffsetBlock.GetOrientedTopSide(gravityDir);
                topOffsetSide.AddHeightOffset(distanceOffsetFromGrid - 1.0f);

                bottomOffsetSide = OffsetBlock.GetOrientedTopSide(-gravityDir);
                bottomOffsetSide.AddHeightOffset(-distanceOffsetFromGrid + 1.0f);

                gridBlocks.Add(OffsetBlock);
            }
        }
    }

    public List<Block> GetGridBlocks()
    {
        return gridBlocks;
    }

    //takes an on grid position and retruns a list of all the subblocks based on that position
    //used for on grid block placement, don't use for in game off grid calculations
    public List<Vector3Int> getSubBlockPositions(Vector3Int mainPosition)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        foreach(ComponentBlockDefinition component in componentBlocks)
        {
            positions.Add(mainPosition + component.offsetFromCenter);
        }

        return positions;
    }
}
