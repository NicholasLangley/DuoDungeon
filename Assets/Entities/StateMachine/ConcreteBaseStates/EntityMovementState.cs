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
        movementLerpTimer = 0;
        _entity.busy = true;
 
        srcPosition = _entity.transform.position;
        //projected grid position, need to add block height if a block occupies the position
        destPosition = _entity.GetProjectedDestinationBlockPosition(_entity.movementDirection);

        //halfway point still need to calculate proper y value with block heights
        halfwayPosition = Vector3.Lerp(srcPosition, destPosition, 0.5f);

        Block srcBlock = _entity.GetCurrentlyOccupiedBlock();
        Block destBlock = _entity.GetBlockFromMap(destPosition);

        //function doesnt care about y differences
        if (srcBlock != null) { halfwayPosition.y = srcBlock.transform.position.y + srcBlock.CalculateAttemptedExitEdgeHeight(destPosition); }
        else { halfwayPosition.y = srcPosition.y; }

        if (destBlock != null) { destPosition.y += destBlock.MidBlockHeight; }

        inSrcBlock = true;
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