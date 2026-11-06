using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//static functions for calculating anything involiving blocks
//replaces the old gameobject based block methods
public static class BlockMath
{
    public static float GetBlockExitHeight(BlockInstance currentBlock, Vector3 entitiesCurrentWorldGridPos, Vector3 entitiesNextPos,
        Vector3 entityUpVector, DownDirection entityDownDir)
    {
        //need the entites relative direction + normal and the forward vector of the block side to determine which side of the blockside its exiting from
        //normal vector should be entities down vector passing from entity throough block
        Vector3 normalVector = -entityUpVector;
        //relative direction is the direction with the vertical direction removed, so which horizontal direction the entity is moving relative to the space its in and its down direction
        Vector3 relativeDirection = entitiesCurrentWorldGridPos - entitiesNextPos;
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

        //the vector pointing from the center of a block side to its forward edge
        //comparing the relativeDirection to the forward vector (using the normal) we can figure out which edge of the block is being exited from
        Vector3 forwardVector;
        
        Vector3 blockOrientedDownVector = Quaternion.Inverse(currentBlock.rotation) * normalVector;
        DownDirection downDir = Entity.ConvertVectorToDownDirection(blockOrientedDownVector);
        switch (downDir)
        {
            //every case except for the forward / back sides have the same forward vector
            //formerly just transform.forward
            default:
            case DownDirection.Yup:
            case DownDirection.Xleft:
            case DownDirection.Xright:
                forwardVector = currentBlock.rotation * Vector3.forward;
                break;
            case DownDirection.Zforward:
                forwardVector = currentBlock.rotation * -Vector3.up;
                break;
            case DownDirection.Zback:
                forwardVector = currentBlock.rotation * Vector3.up;
                break;
        }
        
        float angleFromForwardVector = Vector3.SignedAngle(forwardVector, relativeDirection, normalVector);

        BlockSide side = currentBlock.blockData.GetOrientedTopSide(normalVector, currentBlock.rotation);

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

    public static bool CanEntityEnter(FullGridMoveable fgm, Vector3 currentBlockPos, BlockInstance destBlock, Vector3 destBlockPos)
    {
        DownDirection downDir = fgm.GetCurrentDownDirection();

        float enteringEdgeHeight = CalculateAttemptedEntryEdgeHeight(fgm.transform, downDir) + GetPositionsDownOrientedHeight(destBlockPos, downDir);
        
        //get exit height from currentblock
        BlockInstance? entityCurrentBlock = fgm.map.GetCurrentlyOccupiedBlock(fgm.gameObject, fgm.GetCurrentDownDirection());
        float entityExitHeight;
        if (entityCurrentBlock != null)
        {
            entityExitHeight = GetPositionsDownOrientedHeight(currentBlockPos, downDir) + BlockMath.GetBlockExitHeight((BlockInstance)entityCurrentBlock, currentBlockPos, destBlockPos, fgm.transform.up, fgm.GetCurrentDownDirection());
        }
        else { entityExitHeight = GetPositionsDownOrientedHeight(fgm.transform.position, downDir); }
        
        //if player climbing up a level and still has to climb this block
        if (IsEntityClimbingStackedBlock(destBlockPos, fgm.transform.position, downDir))//transform.position.y - Mathf.Floor(fgm.transform.position.y) >= 0.99f)
        {
            float maxClimb = (fgm.maxStairClimbHeight + entityExitHeight);
            float climbNeeded = enteringEdgeHeight;
            if (climbNeeded <= maxClimb) { return true; }
            //Debug.Log("failed climbing stacked block");
        }
        //same height or greater entry
        else if (IsEntityClimbingOrStayingSameHeight(destBlockPos, fgm.transform.position, downDir))//transform.position.y >= Mathf.Floor(fgm.transform.position.y))
        {
            //Debug.Log(e.transform.position + "\nEntering edgeHeight: " + enteringEdgeHeight + "\nexit stairs: " + (e.maxStairClimbHeight + entityExitHeight));
            if (enteringEdgeHeight <= fgm.maxStairClimbHeight + entityExitHeight) { return true; }
            //Debug.Log("failed climbing Or staying same height");
        }
        //entering from above
        else
        {
            //Debug.Log("exit - climb: " + (entityExitHeight - e.maxStairClimbHeight) + "\n enter: " + enteringEdgeHeight);
            if (entityExitHeight <= enteringEdgeHeight + fgm.maxStairClimbHeight && entityExitHeight >= enteringEdgeHeight - fgm.maxStairClimbHeight) { return true; }
            //Debug.Log("failed entering from above");
        }

        return false;
    }
    
    public static float GetMidBlockHeight(BlockInstance block, Vector3 entityDownVector)
    {
        return block.blockData.GetOrientedTopSide(entityDownVector, block.rotation).centerHeight;
    }
    
}