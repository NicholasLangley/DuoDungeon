using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFallingState : EntityState
{
    float fallingLerpTimer;
    Vector3 startPos;
    Vector3 fallDestination;

    public EntityFallingState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
        
    }


    public override void EnterState()
    {
        _entity.busy = true;
        fallingLerpTimer = 0f;
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
        Vector3 movePos = Vector3.Lerp(startPos, fallDestination, fallingLerpTimer / _entity.movementLerpDuration);
        _entity.transform.position = movePos;

        if (fallingLerpTimer >= _entity.movementLerpDuration) 
        {
            //landed
            if (_entity.isEntityGrounded()) { _stateMachine.changeState(_entity.idleState); }
            //still falling
            else 
            { 
                GetNewFallDestination();
                fallingLerpTimer = 0f;
            }
        }
    }

    void GetNewFallDestination()
    {
        startPos = _entity.transform.position;
        fallDestination = _entity.transform.position;
        fallDestination.y -= 1;
    }
}
