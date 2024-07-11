using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    IMoveable actor;
    MovementDirection direction;

    public MoveCommand(IMoveable actor, MovementDirection dir)
    {
        this.actor = actor;
        direction = dir;
    }

    public override void Execute()
    {
        actor.MoveTo(direction);
    }

    public override void Undo()
    {
        actor.MoveTo(IMoveable.ReverseMovementDirection(direction));
    }
}
