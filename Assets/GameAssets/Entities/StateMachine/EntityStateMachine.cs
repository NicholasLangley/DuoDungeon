using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine
{
    public EntityState currentState { get; set; }

    public void Initialize(EntityState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void changeState(EntityState nextState)
    {
        currentState.ExitState();
        currentState = nextState;
        currentState.EnterState();
    }
}
