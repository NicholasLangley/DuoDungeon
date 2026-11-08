using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockSide;

public class Pushable_FallingState : FullGridMoveableState
{
    float fallingLerpTimer;
    float modifiedFallLerpDuration;
    Vector3 startPos;
    Vector3 fallDestination;

    public Pushable_FallingState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
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

        BlockInstance startingBlock = _fgm.map.GetBlockAtGridPosition(baseBlockPosition, _fgm.gameObject, _fgm.gravityDirection);
        BlockInstance destBlock;
        fallDestination = baseBlockPosition;

        DownDirection downDir = _fgm.GetCurrentDownDirection();

        if (startingBlock != null && startingBlock.blockData.GetOrientedTopSide(_fgm.gravityDirection, startingBlock.rotation).centerType == CenterType.GROUND)
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
            
            destBlock = _fgm.map.GetBlockAtGridPosition(fallDestination, _fgm.gameObject, _fgm.gravityDirection);
        }
        
        if (destBlock != null)
        {
            switch (downDir)
            {
                //YDown
                default:
                    fallDestination.y += BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;
                case DownDirection.Yup:
                    fallDestination.y -= BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;

                case DownDirection.Xright:
                    fallDestination.x -= BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;
                case DownDirection.Xleft:
                    fallDestination.x += BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;

                case DownDirection.Zforward:
                    fallDestination.z -= BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;
                case DownDirection.Zback:
                    fallDestination.z += BlockMath.GetMidBlockHeight((BlockInstance)destBlock, _fgm.gravityDirection);
                    break;
            }
            //stops lesser falls from being slower
            modifiedFallLerpDuration = (Vector3.Distance(fallDestination, startPos)) * modifiedFallLerpDuration;
        }

        
    }
}
