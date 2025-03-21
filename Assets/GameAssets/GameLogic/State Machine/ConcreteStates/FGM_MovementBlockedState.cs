using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_MovementBlockedState : FullGridMoveableState
{
    float movementLerpTimer = 0;
    Vector3 srcPosition;
    Vector3 bumpDest;

    public FGM_MovementBlockedState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        movementLerpTimer = 0;
        srcPosition = _fgm.transform.position;
        _fgm.GetProjectedDestinationBlockPosition(_fgm.movementDirection);
        Vector3 attemptedDestination = _fgm.projectedDestinationBlock;
        bumpDest = Vector3.Lerp(srcPosition, attemptedDestination, 0.5f);

        //modify dest height to match edge of srcblock
        DownDirection downDir = _fgm.GetCurrentDownDirection();
        Block srcBlock = _fgm.map.GetCurrentlyOccupiedBlock(_fgm.transform.position, _fgm.GetCurrentDownDirection());
        if (srcBlock != null) 
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Yup:
                    bumpDest.y = srcBlock.transform.position.y + _fgm.transform.up.y * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _fgm.transform.up, _fgm.GetCurrentDownDirection());
                    break;
                case DownDirection.Xleft:
                case DownDirection.Xright:
                    bumpDest.x = srcBlock.transform.position.x + _fgm.transform.up.x * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _fgm.transform.up, _fgm.GetCurrentDownDirection());
                    break;
                case DownDirection.Zforward:
                case DownDirection.Zback:
                    bumpDest.z = srcBlock.transform.position.z + _fgm.transform.up.z * srcBlock.CalculateAttemptedExitEdgeHeight(attemptedDestination, _fgm.transform.up, _fgm.GetCurrentDownDirection());
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

        _fgm.busy = true;
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
        if (movementLerpTimer <= 0.5f * _fgm.movementLerpDuration)
        {
             movePos = Vector3.Lerp(srcPosition, bumpDest, movementLerpTimer / _fgm.movementLerpDuration);
        }
        else
        {
            movePos = Vector3.Lerp(bumpDest, srcPosition, movementLerpTimer / _fgm.movementLerpDuration);
        }

        _fgm.transform.position = movePos;

        if (movementLerpTimer >= _fgm.movementLerpDuration) { _stateMachine.changeState(_fgm.idleState); }
    }
}
