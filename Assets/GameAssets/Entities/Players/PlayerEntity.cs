using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    protected override void Awake()
    {
        base.Awake();
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }*/

    public override void Attack()
    {
        

        stateMachine.changeState(attackState);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, attackRange, attackMask))
        {
            IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                //heal damage if player is undoing attack
                if (currentlyUndoing) { damageable.Heal(attackDamage); }
                else { damageable.Damage(attackDamage); }

            }
        }
    }
}
