using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCommand : Command
{
    IMoveable actor;
    Pushable  pushable;
    MovementDirection direction;

    public PushCommand(IMoveable actor, Pushable pushable, MovementDirection dir)
    {
        this.actor = actor;
        this.pushable = pushable;
        direction = dir;
    }

    public override void Execute()
    {
        actor.MoveTo(direction);
        pushable.MoveTo(direction);
    }

    public override void Undo()
    {
        actor.MoveTo(IMoveable.ReverseMovementDirection(direction));
        pushable.MoveTo(IMoveable.ReverseMovementDirection(direction));
    }
}
