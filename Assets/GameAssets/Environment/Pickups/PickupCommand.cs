using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCommand : Command
{
    Pickup pickup;
    Entity entity;

    public PickupCommand(Pickup p, Entity e)
    {
        pickup = p;
        entity = e;
    }

    public override void Execute()
    {
        pickup.Activate(entity);
    }

    public override void Undo()
    {
        pickup.Deactivate(entity);
    }
}
