using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFallingState : EntityState
{
    float fallingLerpTimer;
    float modifiedFallLerpDuration;
    Vector3 startPos;
    Vector3 fallDestination;

    public EntityFallingState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
        
    }


    public override void EnterState()
    {
        _entity.busy = true;
        fallingLerpTimer = 0f;
        modifiedFallLerpDuration = _entity.fallLerpDuration;
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
        _entity.transform.position = movePos;

        if (fallingLerpTimer >= _entity.fallLerpDuration) 
        {
            //if player is still midair a new command will be genereated by the player controller
            _stateMachine.changeState(_entity.idleState);
        }
    }

    void GetNewFallDestination()
    {
        startPos = _entity.transform.position;
        Vector3 baseBlockPosition = _entity.transform.position;
        baseBlockPosition.y = Mathf.Floor(baseBlockPosition.y);
        //fallDestination = _entity.transform.position;
        if (_entity.currentlyUndoing) 
        {
            fallDestination = _entity.fallSrcPosition;
            modifiedFallLerpDuration = (fallDestination.y - startPos.y) * modifiedFallLerpDuration;
            return;
        }
        
        baseBlockPosition.y -= 1;
        fallDestination = baseBlockPosition;
        Block destBlock = _entity.GetBlockFromMap(baseBlockPosition);
        if (destBlock != null)
        {
            fallDestination.y += destBlock.MidBlockHeight;
            //stops lesser falls from being slower
            modifiedFallLerpDuration = (startPos.y - fallDestination.y) * modifiedFallLerpDuration;
        }

        
    }
}
