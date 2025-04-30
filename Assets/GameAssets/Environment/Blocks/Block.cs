using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Block : MonoBehaviour, IPlaceable
{
    //baseID identifies the base block (eg full stairs)
    //varientID identifies the version of that block (different materials, meshes, etc.)
    public string listID { get; set; }
    [field: SerializeField] public string baseID  { get; set; }
    [field: SerializeField] public string varientID { get; set; }

    [SerializeField]    
    public BlockSideDefinitions blockSides;

    [SerializeField]
    public bool blocksAllMovement;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {

    }

    //entering from same level as block
    public bool canEntityEnter(FullGridMoveable e)
    {
        if (blocksAllMovement) { return false; }

        DownDirection downDir = e.GetCurrentDownDirection();

        float enteringEdgeHeight = CalculateAttemptedEntryEdgeHeight(e.transform, downDir) + GetPositionsDownOrientedHeight(transform.position, downDir);

        Block entityCurrentBlock = e.map.GetCurrentlyOccupiedBlock(e.gameObject, e.GetCurrentDownDirection());
        float entityExitHeight;
        if (entityCurrentBlock != null)
        {
            entityExitHeight = GetPositionsDownOrientedHeight(entityCurrentBlock.transform.position, downDir) + entityCurrentBlock.CalculateAttemptedExitEdgeHeight(transform.position, e.transform.up, downDir);
        }
        else { entityExitHeight = GetPositionsDownOrientedHeight(e.transform.position, downDir); }

        //if player climbing up a level and still has to climb this block
        if (IsEntityClimbingStackedBlock(transform.position, e.transform.position, downDir))//transform.position.y - Mathf.Floor(e.transform.position.y) >= 0.99f)
        {
            float maxClimb = (e.maxStairClimbHeight + entityExitHeight);
            float climbNeeded = enteringEdgeHeight;
            if (climbNeeded <= maxClimb) { return true; }
        }
        //same height or greater entry
        else if (IsEntityClimbingOrStayingSameHeight(transform.position, e.transform.position, downDir))//transform.position.y >= Mathf.Floor(e.transform.position.y))
        {
            if (enteringEdgeHeight <= e.maxStairClimbHeight + entityExitHeight) { return true; }
        }
        //entering from above
        else
        {
            //Debug.Log("exit - climb: " + (entityExitHeight - e.maxStairClimbHeight) + "\n enter: " + enteringEdgeHeight);
            if (entityExitHeight - e.maxStairClimbHeight <= enteringEdgeHeight ) { return true; }
        }
        

        return false;
    }

    public float CalculateAttemptedEntryEdgeHeight(Transform entityTransform, DownDirection entityDownDir)
    {
        DownDirection blockDownDir = Entity.ConvertVectorToDownDirection(-transform.up);

        //find forward, normal and relative direction vectors of entity
        Vector3 relativeDirection = transform.position - entityTransform.position;
        Vector3 forwardVector;
        Vector3 normalVector = -entityTransform.up;
        switch (entityDownDir)
        {
            default:
            case DownDirection.Yup:
                relativeDirection.y = 0;
                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                relativeDirection.x = 0;
                break;
            case DownDirection.Zforward:
            case DownDirection.Zback:
                relativeDirection.z = 0;
                break;
        }

        Vector3 blockOrientedDownVector = transform.InverseTransformDirection(normalVector);
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);

        switch (downDir)
        {
            default:
            case DownDirection.Yup:
            case DownDirection.Xleft:
            case DownDirection.Xright:
                forwardVector = transform.forward;
                break;
            case DownDirection.Zforward:
                forwardVector = -transform.up;
                break;
            case DownDirection.Zback:
                forwardVector = transform.up;
                break;
        }

        float angleFromForwardVector = Vector3.SignedAngle(forwardVector, relativeDirection, normalVector);

        BlockSide side = GetOrientedTopSide(normalVector);
        //Debug.Log("forward: " + forwardVector + "\n normal: " + normalVector + "\n relative: " + relativeDirection + "\n signedAngle: " + angleFromForwardVector);
        //forward
        if (angleFromForwardVector < 10f && angleFromForwardVector > -10f)
        {
            return side.forwardEdgeHeight;
        }
        //right
        if(angleFromForwardVector >= 10f && angleFromForwardVector <= 120f)
        {
            return side.rightEdgeHeight;
        }
        //back
        if (angleFromForwardVector >= 120f || angleFromForwardVector <= -120f)
        {
            return side.backwardEdgeHeight;
        }
        //left
        return side.leftEdgeHeight;
    }

    public float CalculateAttemptedExitEdgeHeight(Vector3 exitPoint, Vector3 entityUpVector, DownDirection EntityDownDir)
    {
        //find forward, normal and relative direction vectors of entity
        Vector3 relativeDirection = transform.position - exitPoint;
        Vector3 forwardVector;
        Vector3 normalVector = -entityUpVector;
        switch (EntityDownDir)
        {
            default:
            case DownDirection.Yup:
                relativeDirection.y = 0;
                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                relativeDirection.x = 0;
                break;
            case DownDirection.Zforward:
            case DownDirection.Zback:
                relativeDirection.z = 0;
                break;
        }

        Vector3 blockOrientedDownVector = transform.InverseTransformDirection(normalVector);
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);

        switch (downDir)
        {
            default:
            case DownDirection.Yup:
            case DownDirection.Xleft:
            case DownDirection.Xright:
                forwardVector = transform.forward;
                break;
            case DownDirection.Zforward:
                forwardVector = -transform.up;
                break;
            case DownDirection.Zback:
                forwardVector = transform.up;
                break;
        }

        float angleFromForwardVector = Vector3.SignedAngle(forwardVector, relativeDirection, normalVector);

        BlockSide side = GetOrientedTopSide(normalVector);

        //forward
        if (angleFromForwardVector < 10f && angleFromForwardVector > -10f)
        {
            return side.forwardEdgeHeight;
        }
        //right
        if (angleFromForwardVector >= 10f && angleFromForwardVector <= 120f)
        {
            return side.rightEdgeHeight;
        }
        //back
        if (angleFromForwardVector >= 120f || angleFromForwardVector <= -120f)
        {
            return side.backwardEdgeHeight;
        }
        //left
        return side.leftEdgeHeight;
    }

    public float GetMidBlockHeight(Vector3 entityDownVector)
    {
        return GetOrientedTopSide(entityDownVector).centerHeight;
    }

    ///////////
    //HELPERS///
    ////////////
    public BlockSide GetOrientedTopSide(Vector3 entityDownVector)
    {
        Vector3 blockOrientedDownVector = transform.InverseTransformDirection(entityDownVector);
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);

        switch (downDir)
        {
            default:
                //Debug.Log("top");
                return blockSides.topSide;
            case DownDirection.Yup:
                //Debug.Log("bottom");
                return blockSides.bottomSide;
            case DownDirection.Xleft:
                //Debug.Log("right");
                return blockSides.rightSide;
            case DownDirection.Xright:
                //Debug.Log("left");
                return blockSides.leftSide;
            case DownDirection.Zforward:
                //Debug.Log("back");
                return blockSides.backSide;
            case DownDirection.Zback:
               // Debug.Log("front");
                return blockSides.frontSide;
        }
    }

    public static float GetPositionsDownOrientedHeight(Vector3 pos, DownDirection downDir)
    {
        switch (downDir)
        {
            default:
                return pos.y;
            case DownDirection.Yup:
                return pos.y * -1f;
            case DownDirection.Xleft:
                return pos.x;
            case DownDirection.Xright:
                return pos.x * -1f;
            case DownDirection.Zforward:
                return pos.z * -1f;
            case DownDirection.Zback:
                return pos.z;
        }
    }

    bool IsEntityClimbingStackedBlock(Vector3 blockPos, Vector3 entityPos, DownDirection downDir)
    {
        float blockDownOrientedHeight = GetPositionsDownOrientedHeight(blockPos, downDir);
        float entityDownOrientedHeight = GetPositionsDownOrientedHeight(entityPos, downDir);
        
        if (blockDownOrientedHeight - Mathf.Floor(entityDownOrientedHeight) >= 0.99f) { return true; }
        return false;
    }

    bool IsEntityClimbingOrStayingSameHeight(Vector3 blockPos, Vector3 entityPos, DownDirection downDir)
    {
        float blockDownOrientedHeight = GetPositionsDownOrientedHeight(blockPos, downDir);
        float entityDownOrientedHeight = GetPositionsDownOrientedHeight(entityPos, downDir);

        //transform.position.y >= Mathf.Floor(e.transform.position.y))
        if (blockDownOrientedHeight >= Mathf.Floor(entityDownOrientedHeight)) { return true; }
        return false;
    }
}
