using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRotationState : EntityState
{
    float rotationLerpTimer;
    Quaternion startingRotation;
    Quaternion destinationRotation;


    public EntityRotationState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        _entity.busy = true;

        rotationLerpTimer = 0;

        startingRotation = _entity.transform.rotation;
        destinationRotation = Quaternion.Euler(startingRotation.eulerAngles.x, startingRotation.eulerAngles.y + _entity.degreesToRotate, startingRotation.eulerAngles.z);
    }

    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {
        Rotate();
    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public void Rotate()
    {
        rotationLerpTimer += Time.deltaTime;
        _entity.transform.rotation = Quaternion.Lerp(startingRotation, destinationRotation, rotationLerpTimer / _entity.movementLerpDuration);

        if (rotationLerpTimer >= _entity.movementLerpDuration) { _stateMachine.changeState(_entity.idleState); }
    }
}