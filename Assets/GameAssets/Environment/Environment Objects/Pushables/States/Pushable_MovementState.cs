using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//possibly deprecated
//TODO reimplement pushables
/*
public class Pushable_MovementState : FullGridMoveableState
{
    Vector3 srcPosition;
    Vector3 destPosition;
    Vector3 halfwayPosition;

    float movementLerpTimer;

    bool inSrcBlock;
    public Pushable_MovementState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
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

        BlockOLD srcBlockOld = _fgm.map.GetCurrentlyOccupiedBlock(_fgm.gameObject, _fgm.GetCurrentDownDirection());
        BlockOLD destBlockOld = _fgm.map.GetBlockAtGridPosition(Map.GetGridSpace(destPosition), _fgm.gameObject, _fgm.gravityDirection);
        //need to check if the block is being pushed, so we can ignore it
        if(destBlockOld != null)
        {
            Pushable push = destBlockOld.gameObject.transform.parent.gameObject.GetComponent<Pushable>();
            if (push != null) 
            {
                if (push.toBePushedThisRound) { destBlockOld = null; }
            }
        }
        //if (destBlock == null) { Debug.Log(destPosition); }

        DownDirection downDir = _fgm.GetCurrentDownDirection();

        float srcBlockExitHeight = srcBlockOld != null ? srcBlockOld.CalculateAttemptedExitEdgeHeight(destPosition, _fgm.transform.up, downDir) : 0;
        float destBlockEnterHeight = destBlockOld != null ? destBlockOld.CalculateAttemptedEntryEdgeHeight(_fgm.transform, downDir) : 0;

        //function doesnt care about height differences
        if (srcBlockOld != null) 
        {
            //if entity is not grounded it was undoing from a fall, so there is no ground to enter(into)
            if (_fgm.isEntityGrounded()) 
            {
                switch (downDir)
                {
                    case DownDirection.Ydown:
                        halfwayPosition.y = srcBlockOld.worldPosition.y + srcBlockExitHeight;
                        break;
                    case DownDirection.Yup:
                        halfwayPosition.y = srcBlockOld.worldPosition.y - srcBlockExitHeight;
                        break;
                    case DownDirection.Xleft:
                        halfwayPosition.x = srcBlockOld.worldPosition.x + srcBlockExitHeight;
                        break;
                    case DownDirection.Xright:
                        halfwayPosition.x = srcBlockOld.worldPosition.x - srcBlockExitHeight;
                        break;
                    case DownDirection.Zforward:
                        halfwayPosition.z = srcBlockOld.worldPosition.z - srcBlockExitHeight;
                        break;
                    case DownDirection.Zback:
                        halfwayPosition.z = srcBlockOld.worldPosition.z + srcBlockExitHeight;
                        break;
                }
            }
            //not grounded
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
        //if (destBlock != null) 
        //{
        //    Debug.Log("src exit: " + srcBlockExitHeight + "src pos height: " + Block.GetPositionsDownOrientedHeight(srcPosition, downDir));
        //    Debug.Log("dest enter: " + destBlockEnterHeight + "dest pos height: " + Block.GetPositionsDownOrientedHeight(destBlock.gridPosition, downDir));
        //    Debug.Log("TOTAL: " + (srcBlockExitHeight - destBlockEnterHeight + Block.GetPositionsDownOrientedHeight(srcPosition, downDir) - Block.GetPositionsDownOrientedHeight(destBlock.gridPosition, downDir)));
        //}
        if (destBlockOld != null && srcBlockExitHeight + Mathf.Floor(BlockOLD.GetPositionsDownOrientedHeight(srcPosition, downDir)) - (destBlockEnterHeight + BlockOLD.GetPositionsDownOrientedHeight(destBlockOld.worldPosition, downDir)) <= _fgm.maxStairClimbHeight)
        {
            float destBlockHeight = destBlockOld.GetMidBlockHeight(_fgm.gravityDirection);
            //Debug.Log("climbdown within partial: " + destBlockHeight);
            switch (downDir)
            {
                case DownDirection.Ydown:
                    destPosition.y = destBlockOld.worldPosition.y + destBlockHeight;
                    break;
                case DownDirection.Yup:
                    destPosition.y = destBlockOld.worldPosition.y - destBlockHeight;
                    break;
                case DownDirection.Xleft:
                    destPosition.x = destBlockOld.worldPosition.x + destBlockHeight;
                    break;
                case DownDirection.Xright:
                    destPosition.x = destBlockOld.worldPosition.x - destBlockHeight;
                    break;
                case DownDirection.Zforward:
                    destPosition.z = destBlockOld.worldPosition.z - destBlockHeight;
                    break;
                case DownDirection.Zback:
                    destPosition.z = destBlockOld.worldPosition.z + destBlockHeight;
                    break;
            }
        }
        //climbing up to a null block
        else if (destBlockOld == null && Mathf.FloorToInt(BlockOLD.GetPositionsDownOrientedHeight(destPosition, downDir)) - Mathf.FloorToInt(BlockOLD.GetPositionsDownOrientedHeight(halfwayPosition, downDir)) > 0) { Debug.Log("climb up to null"); return; }
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
            if (destBlockOld == null)
            {
                Vector3 belowDestPos = destPosition;
                belowDestPos += _fgm.gravityDirection;
                belowDestPos = Map.GetGridSpace(belowDestPos);
                BlockOLD belowDestBlockOld = _fgm.map.GetBlockAtGridPosition(belowDestPos, _fgm.gameObject, _fgm.gravityDirection);
                if (belowDestBlockOld != null && belowDestBlockOld.GetMidBlockHeight(_fgm.gravityDirection) == 1.0f && Mathf.Abs(BlockOLD.GetPositionsDownOrientedHeight(destPosition, _fgm.GetCurrentDownDirection()) - BlockOLD.GetPositionsDownOrientedHeight(belowDestPos, _fgm.GetCurrentDownDirection()) - 1.0f) <= _fgm.maxStairClimbHeight) 
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
}*/