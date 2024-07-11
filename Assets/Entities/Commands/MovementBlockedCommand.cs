using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBlockedCommand : Command
{
    IMoveable actor;
    MovementDirection direction;
    public MovementBlockedCommand(IMoveable actor, MovementDirection dir)
    {
        this.actor = actor;
        direction = dir;
    }
    public override void Execute()
    {
        actor.FailToMoveTo(direction);
    }

    public override void Undo()
    {
        actor.FailToMoveTo(direction);
    }
}
