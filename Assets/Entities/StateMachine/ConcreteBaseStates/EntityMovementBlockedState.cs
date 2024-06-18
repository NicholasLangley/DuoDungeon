using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementBlockedState : EntityState
{
    float movementLerpTimer = 0;

    public EntityMovementBlockedState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        movementLerpTimer = 0;

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
             movePos = Vector3.Lerp(_entity.srcPosition, _entity.destPosition, movementLerpTimer / _entity.movementLerpDuration);
        }
        else
        {
            movePos = Vector3.Lerp(_entity.destPosition, _entity.srcPosition, movementLerpTimer / _entity.movementLerpDuration);
        }
        
        _entity.transform.position = movePos;

        if (movementLerpTimer >= _entity.movementLerpDuration) { _stateMachine.changeState(_entity.idleState); }
    }
}
