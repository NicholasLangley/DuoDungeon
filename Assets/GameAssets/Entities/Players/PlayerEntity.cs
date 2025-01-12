using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    [SerializeField]
    float attackRange;
    [SerializeField]
    int attackDamage;

    [SerializeField]
    LayerMask attackMask;

    /*private void Awake()
    {
        base.Awake();
    }

    
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
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, attackRange, attackMask))
        {
            IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.Damage(attackDamage);
            }
        }
    }
}
