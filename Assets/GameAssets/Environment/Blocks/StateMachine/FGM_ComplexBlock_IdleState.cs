using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGM_ComplexBlock_IdleState : FGM_IdleState
{
    ComplexBlock complexBlock { get; set; }
    public FGM_ComplexBlock_IdleState(FullGridMoveable fgm, StateMachine stateMachine, ComplexBlock block) : base(fgm, stateMachine)
    {
        complexBlock = block;
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        complexBlock.CalculateGridBlocks();
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
