using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable
{

    //IMoveable variables
    public Vector3 srcPosition { get; set; }
    public Vector3 destPosition { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }


    //ICommandable variables
    [field: SerializeField] public bool busy { get; set; }

    #region State Machine

    public EntityStateMachine stateMachine;

    public EntityIdleState idleState;
    public EntityMovementState movementState;
    public EntityMovementBlockedState movementBlockedState;

    #endregion

    protected void Awake()
    {
        #region Create State Machine and states
        stateMachine = new EntityStateMachine();

        idleState = new EntityIdleState(this, stateMachine);
        movementState = new EntityMovementState(this, stateMachine);
        movementBlockedState = new EntityMovementBlockedState(this, stateMachine);

        #endregion
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //ICommandable
        busy = false;

        #region Initialize State Machine

        stateMachine.Initialize(idleState);

        #endregion
    }

    // Update is called once per frame
    protected void Update()
    {
        stateMachine.currentState.StateUpdate();
    }


    public void MoveTo(Vector3 dest)
    {
        destPosition = dest;
        srcPosition = transform.position;
        stateMachine.changeState(movementState);
    }

    public void FailToMoveTo(Vector3 dest)
    {
        destPosition = dest;
        srcPosition = transform.position;
        stateMachine.changeState(movementBlockedState);
    }


    public Command GetCommand()
    {
        return stateMachine.currentState.StateGetCommand();
    }

    public List<Command> GetCommands() { return null; }
}
