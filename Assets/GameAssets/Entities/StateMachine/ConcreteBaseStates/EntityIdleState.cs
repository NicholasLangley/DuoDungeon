using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdleState : EntityState
{
    public EntityIdleState(Entity entity, StateMachine stateMachine) : base(entity, stateMachine)
    {

    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        _entity.busy = false;
        _entity.projectedDestinationBlock = Map.GetIntVector3(_entity.transform.position);
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
