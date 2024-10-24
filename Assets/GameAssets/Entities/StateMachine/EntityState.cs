using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState
{
    protected Entity _entity;
    protected EntityStateMachine _stateMachine;

    public EntityState(Entity entity, EntityStateMachine stateMachine)
    {
        _entity = entity;
        _stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateUpdate() { }

    public virtual Command StateGetCommand() { return null; }

    public virtual void AnimationTriggerEvent() { }

    public virtual void HandleTriggerCollision(Collider collision) { }
}
