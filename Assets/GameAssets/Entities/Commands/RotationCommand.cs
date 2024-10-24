using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCommand : Command
{
    IMoveable actor;
    float degrees;
    public RotationCommand(IMoveable actor, float deg)
    {
        this.actor = actor;
        degrees = deg;
    }

    public override void Execute()
    {
        actor.RotateBy(degrees);
    }

    public override void Undo()
    {
        actor.RotateBy(-degrees);
    }
}
