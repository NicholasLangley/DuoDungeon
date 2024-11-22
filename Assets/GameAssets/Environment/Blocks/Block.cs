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

public class Block : MonoBehaviour
{
    public int blockID { get; set; }

    //The height in the middle of the block the player will stop at, if = 1, player is simply atop it, but in the space ablove
    [SerializeField] public float MidBlockHeight;

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
    public bool canEntityEnter(Entity e)
    {
        if (blocksAllMovement) { return false; }

        DownDirection downDir = e.GetCurrentDownDirection();

        float enteringEdgeHeight = CalculateAttemptedEntryEdgeHeight(e.transform, downDir) + GetPositionsDownOrientedHeight(transform.position, downDir);

        Block entityCurrentBlock = e.GetCurrentlyOccupiedBlock();
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
            if (entityExitHeight - e.maxStairClimbHeight <= enteringEdgeHeight ) { return true; }
        }
        

        return false;
    }

    public float CalculateAttemptedEntryEdgeHeight(Transform entityTransform, DownDirection downDir)
    {
        Vector3 entryPoint = entityTransform.position;

        //find forward, normal and relative direction vectors of entity
        Vector3 relativeDirection = transform.InverseTransformPoint(entryPoint);
        Vector3 forwardVector;
        Vector3 normalVector = entityTransform.up;
        switch (downDir)
        {
            default:
            case DownDirection.Yup:
                relativeDirection.y = 0;
                forwardVector = transform.forward;

                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                relativeDirection.x = 0;
                forwardVector = transform.forward;
                break;
            case DownDirection.Zforward:
                relativeDirection.z = 0;
                forwardVector = transform.up;
                break;
            case DownDirection.Zback:
                relativeDirection.z = 0;
                forwardVector = transform.forward * -1f;
                break;
        }

        float angleFromForwardVector = Vector3.SignedAngle(forwardVector, relativeDirection, normalVector);

        BlockSide side = GetOrientedTopSide(downDir);

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

        /*Vector3 relativeInts = Map.GetIntVector3(relativeDirection.normalized);

        if(relativeInts.x == 1)
        {
            return rightEdgeHeight;
        }
        else if (relativeInts.x == -1)
        {
            return leftEdgeHeight;
        }
        else if (relativeInts.z == 1)
        {
            return forwardEdgeHeight;
        }
        else
        {
            return backwardEdgeHeight;
        }*/
    }

    public float CalculateAttemptedExitEdgeHeight(Vector3 exitPoint, Vector3 entityUpVector, DownDirection downDir)
    {
        //find forward, normal and relative direction vectors of entity
        Vector3 relativeDirection = transform.InverseTransformPoint(exitPoint);
        Vector3 forwardVector;
        Vector3 normalVector = entityUpVector;
        switch (downDir)
        {
            default:
            case DownDirection.Yup:
                relativeDirection.y = 0;
                forwardVector = transform.forward;

                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                relativeDirection.x = 0;
                forwardVector = transform.forward;
                break;
            case DownDirection.Zforward:
                relativeDirection.z = 0;
                forwardVector = transform.up;
                break;
            case DownDirection.Zback:
                relativeDirection.z = 0;
                forwardVector = transform.forward * -1f;
                break;
        }

        float angleFromForwardVector = Vector3.SignedAngle(forwardVector, relativeDirection, normalVector);

        BlockSide side = GetOrientedTopSide(downDir);

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

        /*Vector3 relativeInts = Map.GetIntVector3(relativeDirection.normalized);

        if (relativeInts.x == 1)
        {
            return rightEdgeHeight;
        }
        else if (relativeInts.x == -1)
        {
            return leftEdgeHeight;
        }
        else if (relativeInts.z == 1)
        {
            return forwardEdgeHeight;
        }
        else
        {
            return backwardEdgeHeight;
        }*/
    }

    public float GetMidBlockHeight(DownDirection downDir)
    {
        switch(downDir)
        {
            default:
                return topSide.centerHeight;
            case DownDirection.Yup:
                return bottomSide.centerHeight;
            case DownDirection.Xleft:
                return rightSide.centerHeight;
            case DownDirection.Xright:
                return leftSide.centerHeight;
            case DownDirection.Zforward:
                return backSide.centerHeight;
            case DownDirection.Zback:
                return frontSide.centerHeight;
        }
    }

    ///////////
    //HELPERS///
    ////////////
    BlockSide GetOrientedTopSide(DownDirection downDir)
    {
        switch (downDir)
        {
            default:
                return topSide;
            case DownDirection.Yup:
                return bottomSide;
            case DownDirection.Xleft:
                return rightSide;
            case DownDirection.Xright:
                return leftSide;
            case DownDirection.Zforward:
                return backSide;
            case DownDirection.Zback:
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
