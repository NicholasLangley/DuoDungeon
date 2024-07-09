using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFallingState : EntityState
{
    float fallingLerpTimer;
    Vector3 startPos;
    Vector3 fallDestination;

    public EntityFallingState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
        
    }


    public override void EnterState()
    {
        _entity.busy = true;
        fallingLerpTimer = 0f;
        GetNewFallDestination();
    }

    public override void ExitState()
    {

    }


    public override void StateUpdate()
    {
        Fall();
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void AnimationTriggerEvent()
    {

    }

    void Fall()
    {
        fallingLerpTimer += Time.deltaTime;
        Vector3 movePos = Vector3.Lerp(startPos, fallDestination, fallingLerpTimer / _entity.fallLerpDuration);
        _entity.transform.position = movePos;

        if (fallingLerpTimer >= _entity.fallLerpDuration) 
        {
            //if player is still midair a new command will be genereated by the player controller
            _stateMachine.changeState(_entity.idleState);
        }
    }

    void GetNewFallDestination()
    {
        startPos = _entity.transform.position;
        fallDestination = _entity.transform.position;
        if (_entity.currentlyUndoing) { fallDestination.y += 1; }
        else { fallDestination.y -= 1; }
    }
}
