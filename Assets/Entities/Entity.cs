using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable, IUndoable
{
    [SerializeField]
    Map map;

    //IMoveable variables
    public MovementDirection movementDirection { get; set; }

    [field: SerializeField] public LayerMask movementCollisionMask { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }

    [field: SerializeField] public bool affectedByGravity { get; set; }
    [field: SerializeField] public float fallLerpDuration { get; set; }

    public float degreesToRotate { get; set; }




    //ICommandable variables
    [field: SerializeField] public bool busy { get; set; }

    #region State Machine

    public EntityStateMachine stateMachine;

    public EntityIdleState idleState;
    public EntityMovementState movementState;
    public EntityMovementBlockedState movementBlockedState;
    public EntityRotationState rotationState;
    public EntityFallingState fallingState;

    #endregion

    //IUndoable
    public bool currentlyUndoing { get; set; }

    protected void Awake()
    {
        #region Create State Machine and states
        stateMachine = new EntityStateMachine();

        idleState = new EntityIdleState(this, stateMachine);
        movementState = new EntityMovementState(this, stateMachine);
        movementBlockedState = new EntityMovementBlockedState(this, stateMachine);
        rotationState = new EntityRotationState(this, stateMachine);
        fallingState = new EntityFallingState(this, stateMachine);

        #endregion
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //ICommandable
        busy = false;
        currentlyUndoing = false;

        #region Initialize State Machine

        stateMachine.Initialize(idleState);

        #endregion
    }

    // Update is called once per frame
    protected void Update()
    {
        stateMachine.currentState.StateUpdate();
    }


    public void MoveTo(MovementDirection dir)
    {
        movementDirection = dir;
        stateMachine.changeState(movementState);
    }

    public void FailToMoveTo(MovementDirection dir)
    {
        movementDirection = dir;
        stateMachine.changeState(movementBlockedState);
    }

    public void Fall()
    {
        stateMachine.changeState(fallingState);
    }

    public Vector3 GetProjectedDestinationPosition(MovementDirection dir)
    {
        Vector3 nextPos = transform.position;

        switch(dir)
        {
            case MovementDirection.FORWARD:
                nextPos += transform.forward;
                break;
            case MovementDirection.BACKWARD:
                nextPos -= transform.forward;
                break;
            case MovementDirection.RIGHT:
                nextPos += transform.right;
                break;
            case MovementDirection.LEFT:
                nextPos -= transform.right;
                break;
            default:
                break;
        }

        //check for entering stairs on current level
        Block blockBeingEntered = map.GetBlock(Map.GetIntVector3(nextPos));
        if (blockBeingEntered != null && blockBeingEntered.GetBlockType() == BLOCK_TYPE.STAIR)
        {
            nextPos.y += (blockBeingEntered as StairBlock).stairHeight;
        }

        return nextPos;


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

    public bool isEntityGrounded()
    {
        Vector3Int groundPos = Map.GetIntVector3(transform.position);
        groundPos.y -= 1;

        Block groundBlock = map.GetBlock(groundPos);

        if (groundBlock == null || !groundBlock.blocksMovement) { return false; }

        return true;
    }

    public void RotateBy(float degrees)
    {
        degreesToRotate = degrees;
        stateMachine.changeState(rotationState);
    }

    //Active decisions by the entity such as to move or attack
    public Command GetCommand()
    {
        return stateMachine.currentState.StateGetCommand();
    }

    //commands that arise from the enemies current environment (sliding on ice, or falling in a hole for example)
    public Command GetPassiveCommand()
    {
        Command cmd = null;

        //check for falling
        if(affectedByGravity)
        {
            if(!isEntityGrounded())
            {
                cmd = new FallCommand(this);
            }
        }

        return cmd;
    }

    public List<Command> GetCommands() { return null; }
}
