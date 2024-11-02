using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCK_TYPE { DEFAULT, STAIR }

public class Block : MonoBehaviour
{
    public int blockID { get; set; }

    //The height in the middle of the block the player will stop at, if = 1, player is simply atop it, but in the space ablove
    [SerializeField] public float MidBlockHeight;

    //The height on each edge of the stair, used for entering/exiting the stair block
    [SerializeField] public float forwardEdgeHeight, backwardEdgeHeight, rightEdgeHeight, leftEdgeHeight;

    [SerializeField]
    public bool blocksAllMovement, isGround, blocksProjectiles, isInbetweenBlock;

    protected BLOCK_TYPE blockType;

    // Start is called before the first frame update
    void Awake()
    {
        blockType = BLOCK_TYPE.DEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {

    }

    public BLOCK_TYPE GetBlockType()
    {
        return blockType;
    }

    //entering from same level as block
    public bool canEntityEnter(Entity e)
    {
        if (blocksAllMovement) { return false; }

        float enteringEdgeHeight = CalculateAttemptedEntryEdgeHeight(e.transform.position) + transform.position.y;

        Block entityCurrentBlock = e.GetCurrentlyOccupiedBlock();
        float entityExitY;
        if (entityCurrentBlock != null)
        {
            entityExitY = entityCurrentBlock.transform.position.y + entityCurrentBlock.CalculateAttemptedExitEdgeHeight(transform.position);
        }
        else { entityExitY = e.transform.position.y; }

        //if player climbing up a level and still has to climb this block
        if (transform.position.y - Mathf.Floor(e.transform.position.y) >= 0.99f)
        {
            float maxClimb = (e.maxStairClimbHeight + entityExitY);
            float climbNeeded = enteringEdgeHeight;
            if (climbNeeded <= maxClimb) { return true; }
        }
        //same height or greater entry
        else if (transform.position.y >= Mathf.Floor(e.transform.position.y))
        {
            if (enteringEdgeHeight <= e.maxStairClimbHeight + entityExitY) { return true; }
        }
        //entering from above
        else
        {
            if (entityExitY - e.maxStairClimbHeight <= enteringEdgeHeight ) { return true; }
        }
        

        return false;
    }

    public float CalculateAttemptedEntryEdgeHeight(Vector3 entryPoint)
    {
        Vector3 relativeDirection = transform.InverseTransformPoint(entryPoint);
        relativeDirection.y = 0;

        Vector3 relativeInts = Map.GetIntVector3(relativeDirection.normalized);

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
        }
    }

    public float CalculateAttemptedExitEdgeHeight(Vector3 exitPoint)
    {
        Vector3 relativeDirection = transform.InverseTransformPoint(exitPoint);
        relativeDirection.y = 0;

        Vector3 relativeInts = Map.GetIntVector3(relativeDirection.normalized);

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
        }
    }
}
