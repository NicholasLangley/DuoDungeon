using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullGridMoveableState : State
{
    protected FullGridMoveable _fgm;

    public FullGridMoveableState(FullGridMoveable moveable, StateMachine stateMachine) : base(stateMachine)
    {
        _fgm = moveable;
    }
}

