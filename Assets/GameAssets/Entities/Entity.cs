using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable, IUndoable, IClimbable
{
    public Map map { get; set; }

    //IMoveable variables
    public MovementDirection movementDirection { get; set; }

    [field: SerializeField] public LayerMask movementCollisionMask { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }

    [field: SerializeField] public bool affectedByGravity { get; set; }
    [field: SerializeField] public float fallLerpDuration { get; set; }

    public Vector3 fallSrcPosition { get; set; }

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

    //IClimbable
    [field: SerializeField] public float maxStairClimbHeight { get; set; }

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

    public void Fall(Vector3 srcPos)
    {
        fallSrcPosition = srcPos;
        stateMachine.changeState(fallingState);
    }

    public Block GetCurrentlyOccupiedBlock ()
    {
        Vector3 gridPosition = transform.position;
        gridPosition.y = Mathf.Floor(gridPosition.y);

        return (map.GetBlock(Map.GetIntVector3(gridPosition)));
    }

    public Vector3 GetProjectedDestinationBlockPosition(MovementDirection dir)
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

        //logical center of current block
        nextPos.y = Mathf.Floor(nextPos.y);

        //if in a block find exit height (if height >= 1 then the player has gone up a y level and we'll check for collision there)
        Block currentBlock = GetCurrentlyOccupiedBlock();
        if (currentBlock != null) { nextPos.y += currentBlock.CalculateAttemptedExitEdgeHeight(nextPos); }
        nextPos.y = Mathf.Floor(nextPos.y);

        //check for stairs going down
        Block straightForwardDestBlock = GetBlockFromMap(nextPos);
        //if straight forwad block is not ground in itself need to check if below block is a staircase for smooth movement
        if(straightForwardDestBlock == null || !straightForwardDestBlock.blocksAllMovement && !straightForwardDestBlock.isGround)
        {
            Vector3 belowDest = nextPos;
            belowDest.y -= 1;
            Block belowDestBlock = GetBlockFromMap(belowDest);
            if(belowDestBlock != null && belowDestBlock.isGround && belowDestBlock.canEntityEnter(this))
            {
                nextPos = belowDest;
            }
        }

        return nextPos;
    }

    public Block GetBlockFromMap(Vector3 pos)
    {
        return map.GetBlock(pos);
    }

    public bool IsDestinationOccupied(Vector3 destinationToCheck)
    {
        Block destinationBlock = map.GetBlock(Map.GetIntVector3(destinationToCheck));
        if (destinationBlock != null && !destinationBlock.canEntityEnter(this)) { return true; }

        if(Physics.Raycast(transform.position, destinationToCheck - transform.position, 1f, movementCollisionMask))
        {
            return true;
        }

        return false;
    }

    public bool isEntityGrounded()
    {
        Block currentBlock = GetCurrentlyOccupiedBlock();
        if (currentBlock != null && currentBlock.isGround) 
        {
            if (transform.position.y - currentBlock.transform.position.y - currentBlock.MidBlockHeight > 0.01f) { return false; }
            return true; 
        }
        else if (currentBlock == null && transform.position.y > Mathf.Floor(transform.position.y)) { return false; }

        Vector3Int groundPos = Map.GetIntVector3(transform.position);
        groundPos.y -= 1;

        Block groundBlock = map.GetBlock(groundPos);

        if (groundBlock == null || !groundBlock.isGround || groundBlock.MidBlockHeight < 0.99f) { return false; }

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
                cmd = new FallCommand(this, transform.position);
            }
        }

        return cmd;
    }

    public List<Command> GetCommands() { return null; }
}
