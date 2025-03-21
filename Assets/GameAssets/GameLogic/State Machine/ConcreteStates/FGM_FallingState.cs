using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_FallingState : FullGridMoveableState
{
    float fallingLerpTimer;
    float modifiedFallLerpDuration;
    Vector3 startPos;
    Vector3 fallDestination;

    public FGM_FallingState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
    {
        
    }


    public override void EnterState()
    {
        _fgm.busy = true;
        fallingLerpTimer = 0f;
        modifiedFallLerpDuration = _fgm.fallLerpDuration;
        GetNewFallDestination();
    }

    public override void ExitState()
    {

    }


    public override void StateUpdate()
    {
        Fall();
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void AnimationTriggerEvent()
    {

    }

    void Fall()
    {
        fallingLerpTimer += Time.deltaTime;
        Vector3 movePos = Vector3.Lerp(startPos, fallDestination, fallingLerpTimer / modifiedFallLerpDuration);
        _fgm.transform.position = movePos;

        if (fallingLerpTimer >= modifiedFallLerpDuration) 
        {
            //if player is still midair a new command will be genereated by the player controller
            _stateMachine.changeState(_fgm.idleState);
        }
    }

    void GetNewFallDestination()
    {
        startPos = _fgm.transform.position;
        Vector3 baseBlockPosition = _fgm.transform.position;
        baseBlockPosition.x = Mathf.Floor(baseBlockPosition.x);
        baseBlockPosition.y = Mathf.Floor(baseBlockPosition.y);
        baseBlockPosition.z = Mathf.Floor(baseBlockPosition.z);
        //fallDestination = _entity.transform.position;
        if (_fgm.currentlyUndoing) 
        {
            fallDestination = _fgm.fallSrcPosition;
            modifiedFallLerpDuration = (Vector3.Distance(fallDestination, startPos)) * modifiedFallLerpDuration;
            return;
        }

        Block startingBlock = _fgm.map.GetBlock(baseBlockPosition);
        Block destBlock;
        fallDestination = baseBlockPosition;

        DownDirection downDir = _fgm.GetCurrentDownDirection();

        if (startingBlock != null && startingBlock.isGround)
        {
            destBlock = startingBlock;
        }
        else
        {
            switch (downDir)
            {
                //YDown
                default:
                    fallDestination.y -= 1;
                    break;
                case DownDirection.Yup:
                    fallDestination.y += 1;
                    break;

                case DownDirection.Xright:
                    fallDestination.x += 1;
                    break;
                case DownDirection.Xleft:
                    fallDestination.x -= 1;
                    break;

                case DownDirection.Zforward:
                    fallDestination.z += 1;
                    break;
                case DownDirection.Zback:
                    fallDestination.z -= 1;
                    break;
            }
            
            destBlock = _fgm.map.GetBlock(fallDestination);
        }
        
        if (destBlock != null)
        {
            switch (downDir)
            {
                //YDown
                default:
                    fallDestination.y += destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;
                case DownDirection.Yup:
                    fallDestination.y -= destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;

                case DownDirection.Xright:
                    fallDestination.x -= destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;
                case DownDirection.Xleft:
                    fallDestination.x += destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;

                case DownDirection.Zforward:
                    fallDestination.z -= destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;
                case DownDirection.Zback:
                    fallDestination.z += destBlock.GetMidBlockHeight(-_fgm.transform.up);
                    break;
            }
            //stops lesser falls from being slower
            modifiedFallLerpDuration = (Vector3.Distance(fallDestination, startPos)) * modifiedFallLerpDuration;
        }

        
    }
}
