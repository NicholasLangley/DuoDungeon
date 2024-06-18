using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBlockedCommand : Command
{
    IMoveable actor;
    Vector3 startingPos;
    Vector3 destPos;
    public MovementBlockedCommand(IMoveable actor, Vector3 start, Vector3 dest)
    {
        this.actor = actor;
        startingPos = start;
        destPos = dest;
    }
    public override void Execute()
    {
        
    }

    public override void Undo()
    {
        
    }
}
