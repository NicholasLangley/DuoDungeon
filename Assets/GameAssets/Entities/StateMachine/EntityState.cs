using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For states that specifically need an entity
public class EntityState : State
{
    protected Entity _entity;

    public EntityState(Entity entity, StateMachine stateMachine) : base(stateMachine)
    {
        _entity = entity;
    }
}
