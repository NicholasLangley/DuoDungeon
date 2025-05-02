using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_MovementState : FullGridMoveableState
{
    Vector3 srcPosition;
    Vector3 destPosition;
    Vector3 halfwayPosition;

    Block srcBlock;
    Block destBlock;
    float movementLerpTimer;

    bool inSrcBlock;
    public FGM_MovementState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        inSrcBlock = true;
        movementLerpTimer = 0;
        _fgm.busy = true;
 
        srcPosition = _fgm.transform.position;
        //projected grid position, need to add block height if a block occupies the position
        _fgm.GetProjectedDestinationBlockPosition(_fgm.movementDirection);
        destPosition = _fgm.projectedDestinationBlock;

        //halfway point still need to calculate proper y value with block heights
        halfwayPosition = Vector3.Lerp(srcPosition, destPosition, 0.5f);

        Block srcBlock = _fgm.map.GetCurrentlyOccupiedBlock(_fgm.gameObject, _fgm.GetCurrentDownDirection());
        Block destBlock = _fgm.map.GetBlockAtGridPosition(destPosition, _fgm.gameObject, _fgm.gravityDirection);
        //if (destBlock == null) { Debug.Log(destPosition); }

        DownDirection downDir = _fgm.GetCurrentDownDirection();

        float srcBlockExitHeight = srcBlock != null ? srcBlock.CalculateAttemptedExitEdgeHeight(destPosition, _fgm.transform.up, downDir) : 0;
        float destBlockEnterHeight = destBlock != null ? destBlock.CalculateAttemptedEntryEdgeHeight(_fgm.transform, downDir) : 0;

        //function doesnt care about height differences
        if (srcBlock != null) 
        {
            //if entity is not grounded it was undoing from a fall, so there is no ground to enter(into)
            if (_fgm.isEntityGrounded()) 
            {
                switch (downDir)
                {
                    case DownDirection.Ydown:
                        halfwayPosition.y = srcBlock.transform.position.y + srcBlockExitHeight;
                        break;
                    case DownDirection.Yup:
                        halfwayPosition.y = srcBlock.transform.position.y - srcBlockExitHeight;
                        break;
                    case DownDirection.Xleft:
                        halfwayPosition.x = srcBlock.transform.position.x + srcBlockExitHeight;
                        break;
                    case DownDirection.Xright:
                        halfwayPosition.x = srcBlock.transform.position.x - srcBlockExitHeight;
                        break;
                    case DownDirection.Zforward:
                        halfwayPosition.z = srcBlock.transform.position.z - srcBlockExitHeight;
                        break;
                    case DownDirection.Zback:
                        halfwayPosition.z = srcBlock.transform.position.z + srcBlockExitHeight;
                        break;
                }
            }
            else 
            {
                switch (downDir)
                {
                    case DownDirection.Ydown:
                    case DownDirection.Yup:
                        halfwayPosition.y = destBlockEnterHeight;
                        break;
                    case DownDirection.Xleft:
                    case DownDirection.Xright:
                        halfwayPosition.x = destBlockEnterHeight;
                        break;
                    case DownDirection.Zforward:
                    case DownDirection.Zback:
                        halfwayPosition.y = destBlockEnterHeight;
                        break;
                }
            }
        }
        //src block == null
        else 
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Yup:
                    halfwayPosition.y = srcPosition.y;
                    break;
                case DownDirection.Xleft:
                case DownDirection.Xright:
                    halfwayPosition.x = srcPosition.x;
                    break;
                case DownDirection.Zforward:
                case DownDirection.Zback:
                    halfwayPosition.z = srcPosition.z;
                    break;
            }
        }

        //If moving into a partial block and able to climb down (not fall)
        if (destBlock != null && Mathf.Abs((srcBlockExitHeight + Mathf.Floor(Block.GetPositionsDownOrientedHeight(_fgm.transform.position, downDir))) - (destBlockEnterHeight + Block.GetPositionsDownOrientedHeight(destBlock.transform.position, downDir))) <= _fgm.maxStairClimbHeight)
        {
            float destBlockHeight = destBlock.GetMidBlockHeight(-_fgm.transform.up);
            Debug.Log("climbdown within partial: " + destBlockHeight);
            switch (downDir)
            {
                case DownDirection.Ydown:
                    destPosition.y = destBlock.transform.position.y + destBlockHeight;
                    break;
                case DownDirection.Yup:
                    destPosition.y = destBlock.transform.position.y - destBlockHeight;
                    break;
                case DownDirection.Xleft:
                    destPosition.x = destBlock.transform.position.x + destBlockHeight;
                    break;
                case DownDirection.Xright:
                    destPosition.x = destBlock.transform.position.x - destBlockHeight;
                    break;
                case DownDirection.Zforward:
                    destPosition.z = destBlock.transform.position.z - destBlockHeight;
                    break;
                case DownDirection.Zback:
                    destPosition.z = destBlock.transform.position.z + destBlockHeight;
                    break;
            }
        }
        //climbing up to a null block
        else if (destBlock == null && Mathf.FloorToInt(Block.GetPositionsDownOrientedHeight(destPosition, downDir)) - Mathf.FloorToInt(Block.GetPositionsDownOrientedHeight(halfwayPosition, downDir)) > 0) { Debug.Log("climb up to null"); return; }
        else
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Yup:
                    destPosition.y = halfwayPosition.y;
                    break;
                case DownDirection.Xleft:
                case DownDirection.Xright:
                    destPosition.x = halfwayPosition.x;
                    break;
                case DownDirection.Zforward:
                case DownDirection.Zback:
                    destPosition.z = halfwayPosition.z;
                    break;
            }
            //check for small step down to full block
            if (destBlock == null)
            {
                Vector3 belowDestPos = destPosition;
                belowDestPos -= _fgm.transform.up;
                Block belowDestBlock = _fgm.map.GetBlockAtGridPosition(belowDestPos, _fgm.gameObject, _fgm.gravityDirection);
                if (belowDestBlock != null && belowDestBlock.GetMidBlockHeight(-_fgm.transform.up) == 1.0f) 
                {
                    switch (downDir)
                    {
                        case DownDirection.Ydown:
                            destPosition.y = Mathf.FloorToInt(destPosition.y + 0.01f);
                            break;
                        case DownDirection.Yup:
                            destPosition.y = Mathf.CeilToInt(destPosition.y - 0.01f);
                            break;
                        case DownDirection.Xleft:
                            destPosition.x = Mathf.FloorToInt(destPosition.x + 0.01f);
                            break;
                        case DownDirection.Xright:
                            destPosition.x = Mathf.CeilToInt(destPosition.x - 0.01f);
                            break;
                        case DownDirection.Zback:
                            destPosition.z = Mathf.FloorToInt(destPosition.z + 0.01f);
                            break;
                        case DownDirection.Zforward:
                            destPosition.z = Mathf.CeilToInt(destPosition.z - 0.01f);
                            break;
                    }
                }
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
            movePos = Vector3.Lerp(srcPosition, halfwayPosition, movementLerpTimer / (_fgm.movementLerpDuration / 2.0f)); 
        }
        //entering destBlock
        else 
        {
            movePos = Vector3.Lerp(halfwayPosition, destPosition, movementLerpTimer / (_fgm.movementLerpDuration / 2.0f));
        }

        _fgm.transform.position = movePos;

        if (movementLerpTimer >= _fgm.movementLerpDuration/ 2.0f ) 
        { 
            if(inSrcBlock)
            {
                movementLerpTimer = 0.0f;
                inSrcBlock = false;
                return;
            }
            _stateMachine.changeState(_fgm.idleState); 
        }
    }
}