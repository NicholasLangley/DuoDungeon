using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;

//contains all the data about a single side of a block
//(the height of its edges (0-1.0 how far the edge is from the far side of the blocks grid space))
//some enums about how the character interacts with entering / leaving by each edge, or if they fall through the block on that side
[System.Serializable]
public class BlockSide
{
    public enum EdgeEnterType { WALK, HOP, NONE }
    public enum CenterType { GROUND, NOT_GROUND }

    [Header("CenterType")]
    public CenterType centerType;
    public float centerHeight;

    [Header("Forward")]
    public EdgeEnterType forwardEdgeType;
    public float forwardEdgeHeight;

    [Header("Backward")]
    public EdgeEnterType backwardEdgeType;
    public float backwardEdgeHeight;

    [Header("Left")]
    public EdgeEnterType leftEdgeType;
    public float leftEdgeHeight;

    [Header("Right")]
    public EdgeEnterType rightEdgeType;
    public float rightEdgeHeight;

    public BlockSide()
    {
        centerType = CenterType.GROUND;
        centerHeight = 0;

        forwardEdgeType = EdgeEnterType.WALK;
        forwardEdgeHeight = 0;

        backwardEdgeType = EdgeEnterType.WALK;
        backwardEdgeHeight = 0;

        leftEdgeType = EdgeEnterType.WALK;
        leftEdgeHeight = 0;

        rightEdgeType = EdgeEnterType.WALK; 
        rightEdgeHeight = 0;
    }
    public void AddHeightOffset(float offset)
    {
        centerHeight += offset;
        forwardEdgeHeight += offset;
        backwardEdgeHeight += offset;
        leftEdgeHeight += offset;
        rightEdgeHeight += offset;
    }

    public bool CheckIfSideExtendsToNextGrid()
    {
        return (centerHeight > 1.0f || forwardEdgeHeight > 1.0f || backwardEdgeHeight > 1.0f || leftEdgeHeight > 1.0f || rightEdgeHeight > 1.0f);
    }

    public void CopySide(BlockSide side)
    {
        centerType = side.centerType;
        centerHeight = side.centerHeight;

        forwardEdgeType = side.forwardEdgeType;
        forwardEdgeHeight = side.forwardEdgeHeight;

        backwardEdgeType = side.backwardEdgeType;
        backwardEdgeHeight = side.backwardEdgeHeight;

        leftEdgeType = side.leftEdgeType;
        leftEdgeHeight = side.leftEdgeHeight;

        rightEdgeType = side.rightEdgeType;
        rightEdgeHeight = side.rightEdgeHeight;
    }
}

//contains every side of a single block, and a helper function for copying from another blocks sides
[System.Serializable]
public class BlockSideDefinitions
{
    public BlockSide topSide, leftSide, rightSide, bottomSide, frontSide, backSide;

    public BlockSideDefinitions()
    {
        topSide = new BlockSide();
        leftSide = new BlockSide();
        rightSide = new BlockSide();
        bottomSide = new BlockSide();
        frontSide = new BlockSide();
        backSide = new BlockSide();
    }
    public void CopyBlockSides(BlockSideDefinitions sides)
    {
        topSide.CopySide(sides.topSide);
        leftSide.CopySide(sides.leftSide);
        rightSide.CopySide(sides.rightSide);
        bottomSide.CopySide(sides.bottomSide);
        frontSide.CopySide(sides.frontSide);
        backSide.CopySide(sides.backSide);
    }
}

//the specific version of a block that contains the mesh and material used and any special data
[CreateAssetMenu(fileName = "blockData", menuName = "Blocks/BlockData")]
public class BlockData : Placeable
{
    //placeable parent has list, base and varient ids
    
    [SerializeField] public BaseBlockData baseBlockData;
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;
    [SerializeField] public List<BlockProperty> properties = new List<BlockProperty>();

    public BlockSide GetOrientedTopSide(Vector3 downVector, Quaternion blockRotation)
    {
        Vector3 blockOrientedDownVector = Quaternion.Inverse(blockRotation) * downVector;
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);
        
        switch (downDir)
        {
            default:
                //Debug.Log("top");
                return baseBlockData.blockSides.topSide;
            case DownDirection.Yup:
                //Debug.Log("bottom");
                return baseBlockData.blockSides.bottomSide;
            case DownDirection.Xleft:
                //Debug.Log("right");
                return baseBlockData.blockSides.rightSide;
            case DownDirection.Xright:
                //Debug.Log("left");
                return baseBlockData.blockSides.leftSide;
            case DownDirection.Zforward:
                //Debug.Log("back");
                return baseBlockData.blockSides.backSide;
            case DownDirection.Zback:
                // Debug.Log("front");
                return baseBlockData.blockSides.frontSide;
        }
    }

    public string GetIdString()
    {
        return listID + ":" + baseID + ":" + varientID;
    }
}