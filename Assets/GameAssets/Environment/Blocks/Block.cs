using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockSide
{
    public enum edgeEnterType { WALK, HOP, NONE }
    public enum centerType { GROUND, NOT_GROUND }

    [Header("CenterType")]
    public centerType type;
    public float centerHeight;

    [Header("Forward")]
    public edgeEnterType forwardEdgeType;
    public float forwardEdgeHeight;

    [Header("Backward")]
    public edgeEnterType backwardEdgeType;
    public float backwardEdgeHeight;

    [Header("Left")]
    public edgeEnterType leftEdgeType;
    public float leftEdgeHeight;

    [Header("Right")]
    public edgeEnterType rightEdgeType;
    public float rightEdgeHeight;
}

public class Block : MonoBehaviour, IPlaceable
{
    //baseID identifieds the base block (eg full stairs)
    //varientID identifies the version of that block (different materials, meshes, etc.)
    public string listID { get; set; }
    [field: SerializeField] public string baseID  { get; set; }
    [field: SerializeField] public string varientID { get; set; }
    public int blockID { get; set; }

    //The height on each edges of the block, used for entering/exiting the stair block
    [SerializeField] public float forwardEdgeHeight, backwardEdgeHeight, rightEdgeHeight, leftEdgeHeight;

    [SerializeField]
    public BlockSide topSide, leftSide, rightSide, bottomSide, frontSide, backSide;

    [SerializeField]
    public bool blocksAllMovement, isGround;

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

        Block entityCurrentBlock = e.map.GetCurrentlyOccupiedBlock(e.transform.position, e.GetCurrentDownDirection());
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
    BlockSide GetOrientedTopSide(Vector3 entityDownVector)
    {
        Vector3 blockOrientedDownVector = transform.InverseTransformDirection(entityDownVector);
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);

        switch (downDir)
        {
            default:
                //Debug.Log("top");
                return topSide;
            case DownDirection.Yup:
                //Debug.Log("bottom");
                return bottomSide;
            case DownDirection.Xleft:
                //Debug.Log("right");
                return rightSide;
            case DownDirection.Xright:
                //Debug.Log("left");
                return leftSide;
            case DownDirection.Zforward:
                //Debug.Log("back");
                return backSide;
            case DownDirection.Zback:
               // Debug.Log("front");
                return frontSide;
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
