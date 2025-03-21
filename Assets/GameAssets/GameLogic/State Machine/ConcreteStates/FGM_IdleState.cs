using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_IdleState : FullGridMoveableState
{
    public FGM_IdleState(FullGridMoveable fgm, StateMachine stateMachine) : base(fgm, stateMachine)
    {

    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        _fgm.busy = false;
        _fgm.projectedDestinationBlock = Map.GetIntVector3(_fgm.transform.position);
    }

    public override void ExitState()
    {

    }

    public override void StateUpdate()
    {

    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public override Command StateGetCommand()
    {
        return null;
    }
}
