using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOLD : PlaceableMonoBehaviour
{
    [SerializeField]    
    public BlockSideDefinitions blockSides;

    //[SerializeField]
    //public bool blocksAllMovement;
    
    public Vector3Int gridPosition, worldPosition;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3Int pos)
    {
        gridPosition = Map.WorldToHalfGrid(pos);
        worldPosition = pos;
    }
    
    public bool canEntityEnter(FullGridMoveable e)
    {
        //if (blocksAllMovement) { Debug.Log("blocks all movement"); return false; }

        DownDirection downDir = e.GetCurrentDownDirection();

        float enteringEdgeHeight = CalculateAttemptedEntryEdgeHeight(e.transform, downDir) + GetPositionsDownOrientedHeight(worldPosition, downDir);

        BlockOLD entityCurrentBlockOld = e.map.GetCurrentlyOccupiedBlock(e.gameObject, e.GetCurrentDownDirection());
        float entityExitHeight;
        if (entityCurrentBlockOld != null)
        {
            entityExitHeight = GetPositionsDownOrientedHeight(entityCurrentBlockOld.worldPosition, downDir) + entityCurrentBlockOld.CalculateAttemptedExitEdgeHeight(worldPosition, e.transform.up, downDir);
        }
        else { entityExitHeight = GetPositionsDownOrientedHeight(e.transform.position, downDir); }
        //if player climbing up a level and still has to climb this block
        if (IsEntityClimbingStackedBlock(worldPosition, e.transform.position, downDir))//transform.position.y - Mathf.Floor(e.transform.position.y) >= 0.99f)
        {
            float maxClimb = (e.maxStairClimbHeight + entityExitHeight);
            float climbNeeded = enteringEdgeHeight;
            if (climbNeeded <= maxClimb) { return true; }
            //Debug.Log("failed climbing stacked block");
        }
        //same height or greater entry
        else if (IsEntityClimbingOrStayingSameHeight(worldPosition, e.transform.position, downDir))//transform.position.y >= Mathf.Floor(e.transform.position.y))
        {
            //Debug.Log(e.transform.position + "\nEntering edgeHeight: " + enteringEdgeHeight + "\nexit stairs: " + (e.maxStairClimbHeight + entityExitHeight));
            if (enteringEdgeHeight <= e.maxStairClimbHeight + entityExitHeight) { return true; }
            //Debug.Log("failed climbing Or staying same height");
        }
        //entering from above
        else
        {
            //Debug.Log("exit - climb: " + (entityExitHeight - e.maxStairClimbHeight) + "\n enter: " + enteringEdgeHeight);
            if (entityExitHeight <= enteringEdgeHeight + e.maxStairClimbHeight && entityExitHeight >= enteringEdgeHeight - e.maxStairClimbHeight) { return true; }
            //Debug.Log("failed entering from above");
        }

        return false;
    }

    public float CalculateAttemptedEntryEdgeHeight(Transform entityTransform, DownDirection entityDownDir)
    {
        //find forward, normal and relative direction vectors of entity
        Vector3 relativeDirection = worldPosition - entityTransform.position;
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
        Vector3 relativeDirection = worldPosition - exitPoint;
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
