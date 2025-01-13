using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    Entity entity;

    public AttackCommand(Entity e)
    {
        entity = e;
    }

    public override void Execute()
    {
        entity.Attack();
    }

    public override void Undo()
    {
        //entity will know it is undoing and change logic accordingly
        entity.Attack();
    }
}
