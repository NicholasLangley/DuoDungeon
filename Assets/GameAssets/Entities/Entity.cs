using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : FullGridMoveable, IDamageable
{


    #region State Machine

    //unique entites may override this
    public EntityAttackState attackState;

    #endregion

    #region IDamageable Variables
    [field: SerializeField] public int maxHealth { get; set; }
    public int currentHealth { get; set; }

    #endregion

    #region Attacking Variables

    [SerializeField]
    protected float attackRange;
    [SerializeField] 
    public float attackDuration;
    [SerializeField]
    protected int attackDamage;

    [SerializeField]
    protected LayerMask attackMask;

    #endregion

    protected override void Awake()
    {
        #region Create State Machine and states
        stateMachine = new EntityStateMachine();

        idleState = new EntityIdleState(this, stateMachine);
        movementState = new EntityMovementState(this, stateMachine);
        movementBlockedState = new EntityMovementBlockedState(this, stateMachine);
        rotationState = new EntityRotationState(this, stateMachine);
        fallingState = new EntityFallingState(this, stateMachine);
        //unique entities may override this
        attackState = new EntityAttackState(this, stateMachine);

        #endregion
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //ICommandable
        busy = false;

        //IUndoable
        currentlyUndoing = false;

        //IDamageable
        currentHealth = maxHealth;

        #region Initialize State Machine

        stateMachine.Initialize(idleState);

        #endregion
    }

    // Update is called once per frame
    protected void Update()
    {
        stateMachine.currentState.StateUpdate();
    }


    #region IDamageable

    public void Damage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log(gameObject.name + " has " + currentHealth + " hp");
        if (currentHealth <= 0)
        {
            Die();
        }

        //TODO animations
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        //TODO animations
    }

    void Die()
    {
        //TODO
        Debug.Log(gameObject.name + " has died");
    }

    #endregion

    #region Attacking

    public virtual void Attack() { Debug.Log("This entity has not overridden the default attack"); }

    #endregion
}
