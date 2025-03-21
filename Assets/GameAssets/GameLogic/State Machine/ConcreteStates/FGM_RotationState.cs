using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_RotationState : FullGridMoveableState
{
    float rotationLerpTimer;
    Quaternion startingRotation;
    Quaternion destinationRotation;


    public FGM_RotationState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        _fgm.busy = true;

        rotationLerpTimer = 0;

        startingRotation = _fgm.transform.rotation;
        destinationRotation = Quaternion.Euler(startingRotation.eulerAngles.x, startingRotation.eulerAngles.y + _fgm.degreesToRotate, startingRotation.eulerAngles.z);
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
        _fgm.transform.rotation = Quaternion.Lerp(startingRotation, destinationRotation, rotationLerpTimer / _fgm.movementLerpDuration);

        if (rotationLerpTimer >= _fgm.movementLerpDuration) { _stateMachine.changeState(_fgm.idleState); }
    }
}