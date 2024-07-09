using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCommand : Command
{
    IMoveable actor;
    public FallCommand(IMoveable actor)
    {
        this.actor = actor;
    }
    public override void Execute()
    {
        actor.Fall();
    }

    public override void Undo()
    {
        //falling state will detect undoing and raise actor instead 
        actor.Fall();
    }
}
