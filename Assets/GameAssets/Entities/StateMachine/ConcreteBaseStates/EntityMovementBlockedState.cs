using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementBlockedState : EntityState
{
    float movementLerpTimer = 0;
    Vector3 srcPosition;
    Vector3 bumpDest;

    public EntityMovementBlockedState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        movementLerpTimer = 0;
        srcPosition = _entity.transform.position;
        _entity.GetProjectedDestinationBlockPosition(_entity.movementDirection);
        Vector3 attemptedDestination = _entity.projectedDestinationBlock;
        bumpDest = Vector3.Lerp(srcPosition, attemptedDestination, 0.5f);

        //modify dest height to match edge of srcblock
        DownDirection downDir = _entity.GetCurrentDownDirection();
        Block srcBlock = _entity.GetCurrentlyOccupiedBlock();
        if (srcBlock != null) 
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Yup:
                    bumpDest.y = srcBlock.transform.position.y + _entity.transform.up.y * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _entity.transform.up, _entity.GetCurrentDownDirection());
                    break;
                case DownDirection.Xleft:
                case DownDirection.Xright:
                    bumpDest.x = srcBlock.transform.position.x + _entity.transform.up.x * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _entity.transform.up, _entity.GetCurrentDownDirection());
                    break;
                case DownDirection.Zforward:
                case DownDirection.Zback:
                    bumpDest.z = srcBlock.transform.position.z + _entity.transform.up.z * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _entity.transform.up, _entity.GetCurrentDownDirection());
                    break;
            }
             
        }
        else 
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Yup:
                    bumpDest.y = srcPosition.y;
                    break;
                case DownDirection.Xleft:
                case DownDirection.Xright:
                    bumpDest.x = srcPosition.x;
                    break;
                case DownDirection.Zforward:
                case DownDirection.Zback:
                    bumpDest.z = srcPosition.z;
                    break;
            } 
        }

        _entity.busy = true;
    }

    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {
        FailToMove();
    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public void FailToMove()
    {
        movementLerpTimer += Time.deltaTime;
        Vector3 movePos;
        //move towards obstacle
        if (movementLerpTimer <= 0.5f * _entity.movementLerpDuration)
        {
             movePos = Vector3.Lerp(srcPosition, bumpDest, movementLerpTimer / _entity.movementLerpDuration);
        }
        else
        {
            movePos = Vector3.Lerp(bumpDest, srcPosition, movementLerpTimer / _entity.movementLerpDuration);
        }
        
        _entity.transform.position = movePos;

        if (movementLerpTimer >= _entity.movementLerpDuration) { _stateMachine.changeState(_entity.idleState); }
    }
}
