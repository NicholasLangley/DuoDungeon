using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IMoveable, ICommandable, IUndoable, IClimbable
{
    public Map map { get; set; }
    public GameController gameController { get; set; }

    //IMoveable variables
    public MovementDirection movementDirection { get; set; }

    [field: SerializeField] public LayerMask movementCollisionMask { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }

    [field: SerializeField] public bool affectedByGravity { get; set; }
    [field: SerializeField] public float fallLerpDuration { get; set; }

    public Vector3 fallSrcPosition { get; set; }

    public float degreesToRotate { get; set; }

    public Vector3Int projectedDestinationBlock {get; set;}

    public Vector3Int GetCurrentBlockPosition() 
    { 
        Vector3 currentVec3 = transform.position;
        currentVec3.y = Mathf.FloorToInt(currentVec3.y);
        return Map.GetIntVector3(currentVec3); 
    }


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

    public DownDirection GetCurrentDownDirection()
    {
        return ConvertVectorToDownDirection(-1 * transform.up);
    }

    public static DownDirection ConvertVectorToDownDirection(Vector3 downVec)
    {
        Vector3 down = Map.GetIntVector3(downVec);

        if (down.y == -1) { return DownDirection.Ydown; }
        if (down.y == 1) { return DownDirection.Yup; }

        if (down.x == -1) { return DownDirection.Xleft; }
        if (down.x == 1) { return DownDirection.Xright; }

        if (down.z == -1) { return DownDirection.Zback; }
        return DownDirection.Zforward;
    }

    public Block GetCurrentlyOccupiedBlock ()
    {
        Vector3 gridPosition = transform.position;

        DownDirection downDir = GetCurrentDownDirection();

        switch(downDir)
        {
            case DownDirection.Ydown:
                gridPosition.y = Mathf.Floor(gridPosition.y);
                break;
            case DownDirection.Yup:
                gridPosition.y = Mathf.Ceil(gridPosition.y);
                break;
            case DownDirection.Xleft:
                gridPosition.x = Mathf.Ceil(gridPosition.x);
                break;
            case DownDirection.Xright:
                gridPosition.x = Mathf.Floor(gridPosition.x);
                break;
            case DownDirection.Zforward:
                gridPosition.z = Mathf.Floor(gridPosition.z);
                break;
            case DownDirection.Zback:
                gridPosition.z = Mathf.Ceil(gridPosition.z);
                break;
        }

        return (map.GetBlock(Map.GetIntVector3(gridPosition)));
    }

    public void GetProjectedDestinationBlockPosition(MovementDirection dir)
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

        DownDirection downDir = GetCurrentDownDirection();

        //remove height to get logical center of current block
        switch (downDir)
        {
            case DownDirection.Ydown:
            case DownDirection.Yup:
                nextPos.y = Mathf.Floor(nextPos.y);
                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                nextPos.x = Mathf.Floor(nextPos.x);
                break;
            case DownDirection.Zforward:
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z);
                break;
        } 


        //if in a partial block find exit height (if height >= 1 then the player has gone up a level and we'll check for collision there)
        Block currentBlock = GetCurrentlyOccupiedBlock();
        float exitHeight = currentBlock != null ? currentBlock.CalculateAttemptedExitEdgeHeight(nextPos, transform.up, GetCurrentDownDirection()) : 0;
        nextPos += transform.up * exitHeight;
        switch (downDir)
        {
            case DownDirection.Ydown:
            case DownDirection.Yup:
                nextPos.y = Mathf.Floor(nextPos.y);
                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                nextPos.x = Mathf.Floor(nextPos.x);
                break;
            case DownDirection.Zforward:
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z);
                break;
        }

        //check for stairs going down
        Block straightForwardDestBlock = GetBlockFromMap(nextPos);
        //if straight forwad block is not ground in itself need to check if below block is a staircase for smooth movement
        if(straightForwardDestBlock == null || !straightForwardDestBlock.blocksAllMovement && !straightForwardDestBlock.isGround)
        {
            Vector3 belowDest = nextPos - transform.up;

            Block belowDestBlock = GetBlockFromMap(belowDest);
            if(belowDestBlock != null && belowDestBlock.isGround && belowDestBlock.canEntityEnter(this))
            {
                nextPos = belowDest;
                nextPos += transform.up * belowDestBlock.GetMidBlockHeight(-transform.up);
            }
        }
        //block exists and is full (player will try to climb ontop of it if possible)
        else if(straightForwardDestBlock != null && exitHeight < 1.0f)
        {
            float entryHeight = straightForwardDestBlock.CalculateAttemptedEntryEdgeHeight(transform, GetCurrentDownDirection());
            if (entryHeight > 0.99f && (entryHeight - exitHeight) < maxStairClimbHeight)
            { 
                 nextPos += transform.up;
            }
        }

        switch (downDir)
        {
            case DownDirection.Ydown:
            case DownDirection.Yup:
                nextPos.y = Mathf.Floor(nextPos.y);
                break;
            case DownDirection.Xleft:
            case DownDirection.Xright:
                nextPos.x = Mathf.Floor(nextPos.x);
                break;
            case DownDirection.Zforward:
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z);
                break;
        }
        projectedDestinationBlock = Map.GetIntVector3(nextPos);
    }

    public Block GetBlockFromMap(Vector3 pos)
    {
        return map.GetBlock(pos);
    }

    public bool IsDestinationOccupied(Vector3Int destinationToCheck)
    {
        //block collision
        Block destinationBlock = GetBlockFromMap(destinationToCheck);
        if (destinationBlock != null)
        {
            if (!destinationBlock.canEntityEnter(this)) { return true; }
            if(destinationBlock.GetMidBlockHeight(-transform.up) > 0.25)
            {
                Vector3 headCheckPos = destinationToCheck;
                headCheckPos += transform.up;
                Block headCheckBlock = GetBlockFromMap(Map.GetIntVector3(headCheckPos));
                if (headCheckBlock != null) { return true; }
            }
        }

        //check for entities
        Entity blockingEntity = gameController.GetEntityAtPosition(destinationToCheck);
        if (blockingEntity != null) { return true; }
        return false;
    }

    public bool isEntityGrounded()
    {
        DownDirection downDir = GetCurrentDownDirection();

        float downDirectionEntityHeight = Block.GetPositionsDownOrientedHeight(transform.position, downDir);

        Block currentBlock = GetCurrentlyOccupiedBlock();
        if (currentBlock != null && currentBlock.isGround) 
        {
            float blockDownDirectionHeight = Block.GetPositionsDownOrientedHeight(currentBlock.transform.position, downDir);

            //In block but floating above it and need to fall
            if (downDirectionEntityHeight - blockDownDirectionHeight - currentBlock.GetMidBlockHeight(-transform.up) > 0.01f) { Debug.Log("floating in block"); return false; }
            return true; 
        }
        //floating in empty block
        else if (currentBlock == null && downDirectionEntityHeight > Mathf.Floor(downDirectionEntityHeight)) { Debug.Log("fall"); return false; }

        //check if standing on a block below current one
        Vector3Int groundPos = Map.GetIntVector3(transform.position);
        switch (downDir)
        {
            //YDown
            default:
                groundPos.y -= 1;
                break;
            case DownDirection.Yup:
                groundPos.y += 1;
                break;
            case DownDirection.Xright:
                groundPos.x += 1;
                break;
            case DownDirection.Xleft:
                groundPos.x -= 1;
                break;

            case DownDirection.Zforward:
                groundPos.z += 1;
                break;
            case DownDirection.Zback:
                groundPos.z -= 1;
                break;
        }

        Block groundBlock = map.GetBlock(groundPos);

        if (groundBlock == null || !groundBlock.isGround || groundBlock.GetMidBlockHeight(-transform.up) < 0.99f) { Debug.Log("fall no block"); return false; }
        
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
