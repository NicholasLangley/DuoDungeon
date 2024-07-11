using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementState : EntityState
{
    Vector3 srcPosition;
    Vector3 destPosition;
    float movementLerpTimer;
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
        destPosition = _entity.GetProjectedDestinationPosition(_entity.movementDirection);
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
        Vector3 movePos = Vector3.Lerp(srcPosition, destPosition, movementLerpTimer / _entity.movementLerpDuration);
        _entity.transform.position = movePos;

        if (movementLerpTimer >= _entity.movementLerpDuration) { _stateMachine.changeState(_entity.idleState); }
    }
}