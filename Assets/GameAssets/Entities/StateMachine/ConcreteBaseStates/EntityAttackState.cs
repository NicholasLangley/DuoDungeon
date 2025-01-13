using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttackState : EntityState
{
    float attackTimer, attackDuration;
    Vector3 srcPosition;

    public EntityAttackState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {

    }

    public override void EnterState()
    {
        attackTimer = 0;
        attackDuration = _entity.attackDuration;
        srcPosition = _entity.transform.position;

        _entity.busy = true;
    }

    public override void ExitState()
    {
        _entity.transform.position = srcPosition;
    }

    public override void StateUpdate()
    {
        Attack();
    }

    public override Command StateGetCommand()
    {
        return null;
    }

    public override void HandleTriggerCollision(Collider collision)
    {

    }

    void Attack()
    {
        attackTimer += Time.deltaTime;

        Vector3 nextPos = srcPosition + (_entity.transform.up * Mathf.Sin(Mathf.PI * attackTimer / attackDuration) / 3f);
        _entity.transform.position = nextPos;

        if (attackTimer >= attackDuration) { _stateMachine.changeState(_entity.idleState); }
    }

}
