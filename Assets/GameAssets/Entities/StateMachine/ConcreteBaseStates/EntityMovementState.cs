using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementState : EntityState
{
    Vector3 srcPosition;
    Vector3 destPosition;
    Vector3 halfwayPosition;

    Block srcBlock;
    Block destBlock;
    float movementLerpTimer;

    bool inSrcBlock;
    public EntityMovementState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        inSrcBlock = true;
        movementLerpTimer = 0;
        _entity.busy = true;
 
        srcPosition = _entity.transform.position;
        //projected grid position, need to add block height if a block occupies the position
        _entity.GetProjectedDestinationBlockPosition(_entity.movementDirection);
        destPosition = _entity.projectedDestinationBlock;

        //halfway point still need to calculate proper y value with block heights
        halfwayPosition = Vector3.Lerp(srcPosition, destPosition, 0.5f);

        Block srcBlock = _entity.GetCurrentlyOccupiedBlock();
        Block destBlock = _entity.GetBlockFromMap(destPosition);

        float srcBlockExitHeight = srcBlock != null ? srcBlock.CalculateAttemptedExitEdgeHeight(destPosition) : 0;
        float destBlockEnterHeight = destBlock != null ? destBlock.CalculateAttemptedEntryEdgeHeight(srcPosition) : 0;

        //function doesnt care about y differences
        if (srcBlock != null) 
        {
            //if entity is not grounded it was undoing from a fall, so there is no ground to enter(into)
            if (_entity.isEntityGrounded()) { halfwayPosition.y = srcBlock.transform.position.y + srcBlockExitHeight; }
            else { halfwayPosition.y = destBlockEnterHeight; }
        }
        else { halfwayPosition.y = srcPosition.y; }

        //If moving into a partial block and able to climb down (not fall)
        if (destBlock != null && Mathf.Abs((srcBlockExitHeight + Mathf.Floor(_entity.transform.position.y)) - (destBlockEnterHeight + destBlock.transform.position.y)) <= _entity.maxStairClimbHeight) { destPosition.y += destBlock.MidBlockHeight; }
        else if (destBlock == null && Mathf.FloorToInt(destPosition.y) - Mathf.FloorToInt(halfwayPosition.y) > 0) { return; } //climbing up to a null block
        else
        {
            destPosition.y = halfwayPosition.y;
            //check for small step down to full block
            if (destBlock == null)
            {
                Vector3 belowDestPos = destPosition;
                belowDestPos.y -= 1;
                Block belowDestBlock = _entity.GetBlockFromMap(belowDestPos);
                if (belowDestBlock != null && belowDestBlock.MidBlockHeight == 1.0f) { destPosition.y = Mathf.FloorToInt(destPosition.y); }
            }
        }
    }

    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {
        Move();
    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public void Move()
    {
        movementLerpTimer += Time.deltaTime;

        Vector3 movePos;
        //leaving src block
        if(inSrcBlock)
        {
            movePos = Vector3.Lerp(srcPosition, halfwayPosition, movementLerpTimer / (_entity.movementLerpDuration / 2.0f)); 
        }
        //entering destBlock
        else 
        {
            movePos = Vector3.Lerp(halfwayPosition, destPosition, movementLerpTimer / (_entity.movementLerpDuration / 2.0f));
        }

        _entity.transform.position = movePos;

        if (movementLerpTimer >= _entity.movementLerpDuration/ 2.0f ) 
        { 
            if(inSrcBlock)
            {
                movementLerpTimer = 0.0f;
                inSrcBlock = false;
                return;
            }
            _stateMachine.changeState(_entity.idleState); 
        }
    }
}