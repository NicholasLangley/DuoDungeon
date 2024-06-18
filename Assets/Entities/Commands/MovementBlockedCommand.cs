using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBlockedCommand : Command
{
    IMoveable actor;
    Vector3 destPos;
    public MovementBlockedCommand(IMoveable actor, Vector3 dest)
    {
        this.actor = actor;
        destPos = dest;
    }
    public override void Execute()
    {
        actor.FailToMoveTo(destPos);
    }

    public override void Undo()
    {
        actor.FailToMoveTo(destPos);
    }
}
