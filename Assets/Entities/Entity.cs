using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable
{
    [SerializeField]
    Map map;

    //IMoveable variables
    public Vector3 srcPosition { get; set; }
    public Vector3 destPosition { get; set; }

    [field: SerializeField] public LayerMask movementCollisionMask { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }

    public float degreesToRotate { get; set; }


    //ICommandable variables
    [field: SerializeField] public bool busy { get; set; }

    #region State Machine

    public EntityStateMachine stateMachine;

    public EntityIdleState idleState;
    public EntityMovementState movementState;
    public EntityMovementBlockedState movementBlockedState;
    public EntityRotationState rotationState;

    #endregion

    protected void Awake()
    {
        #region Create State Machine and states
        stateMachine = new EntityStateMachine();

        idleState = new EntityIdleState(this, stateMachine);
        movementState = new EntityMovementState(this, stateMachine);
        movementBlockedState = new EntityMovementBlockedState(this, stateMachine);
        rotationState = new EntityRotationState(this, stateMachine);

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

    public bool IsDestinationOccupied(Vector3 destinationToCheck)
    {
        Block destinationBlock = map.GetBlock(Map.GetIntVector3(destinationToCheck));
        if (destinationBlock != null && destinationBlock.blocksMovement) { return true; }

        if(Physics.Raycast(transform.position, destinationToCheck - transform.position, 1f, movementCollisionMask))
        {
            return true;
        }

        return false;
    }

    public void RotateBy(float degrees)
    {
        degreesToRotate = degrees;
        stateMachine.changeState(rotationState);
    }

    public Command GetCommand()
    {
        return stateMachine.currentState.StateGetCommand();
    }

    public List<Command> GetCommands() { return null; }
}
