using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCommand : Command
{
    IMoveable actor;
    Vector3 srcPosition;
    public FallCommand(IMoveable actor, Vector3 src)
    {
        this.actor = actor;
        srcPosition = src;
    }
    public override void Execute()
    {
        actor.Fall(srcPosition);
    }

    public override void Undo()
    {
        //falling state will detect undoing and raise actor instead 
        actor.Fall(srcPosition);
    }
}
