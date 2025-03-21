using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateUpdate() { }

    public virtual Command StateGetCommand() { return null; }

    public virtual void AnimationTriggerEvent() { }

    public virtual void HandleTriggerCollision(Collider collision) { }
}
