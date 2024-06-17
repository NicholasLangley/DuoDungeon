using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    IMoveable actor;
    Vector3 startingPos;
    Vector3 destPos;

    public MoveCommand(IMoveable actor, Vector3 start, Vector3 dest)
    {
        this.actor = actor;
        startingPos = start;
        destPos = dest;
    }

    public override void Execute()
    {
        actor.MoveTo(destPos);
    }

    public override void Undo()
    {
        actor.MoveTo(startingPos);
    }
}
