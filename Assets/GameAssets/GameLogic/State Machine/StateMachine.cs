using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State currentState { get; set; }

    public void Initialize(State startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void changeState(State nextState)
    {
        currentState.ExitState();
        currentState = nextState;
        currentState.EnterState();
    }
}
