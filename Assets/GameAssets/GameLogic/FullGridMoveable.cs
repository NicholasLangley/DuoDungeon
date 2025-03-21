using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditorInternal;
using UnityEngine;

public abstract class FullGridMoveable : MonoBehaviour, IMoveable, IClimbable, ICommandable, IUndoable
{
    public Map map { get; set; }
    public GameController gameController { get; set; }

    #region IMoveable Variables
    //IMoveable variables
    public MovementDirection movementDirection { get; set; }

    [field: SerializeField] public LayerMask movementCollisionMask { get; set; }

    [field: SerializeField] public float movementLerpDuration { get; set; }

    [field: SerializeField] public bool affectedByGravity { get; set; }
    [field: SerializeField] public float fallLerpDuration { get; set; }

    public Vector3 fallSrcPosition { get; set; }

    public float degreesToRotate { get; set; }

    public Vector3Int projectedDestinationBlock { get; set; }

    #endregion

    #region IClimabable Variables

    //IClimbable
    [field: SerializeField] public float maxStairClimbHeight { get; set; }

    #endregion

    #region IUndoable Variables
    //IUndoable
    public bool currentlyUndoing { get; set; }

    #endregion

    #region ICommandable variables
    //ICommandable variables
    [field: SerializeField] public bool busy { get; set; }

    #endregion

    #region State Machine

    public StateMachine stateMachine;

    public State idleState;
    public State movementState;
    public State movementBlockedState;
    public State rotationState;
    public State fallingState;

    #endregion

    ///////////////////////////////////////////////
    // Start is called before the first frame update
    protected abstract void Awake();
    protected virtual void Start()
    {
        //ICommandable
        busy = false;

        //IUndoable
        currentlyUndoing = false;

        #region Initialize State Machine

        stateMachine.Initialize(idleState);

        #endregion
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        stateMachine.currentState.StateUpdate();
    }
    //////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////

    #region IMoveable
    public Vector3Int GetCurrentBlockPosition()
    {
        Vector3 currentVec3 = transform.position;
        currentVec3.y = Mathf.FloorToInt(currentVec3.y);
        return Map.GetIntVector3(currentVec3);
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

    public void RotateBy(float degrees)
    {
        degreesToRotate = degrees;
        stateMachine.changeState(rotationState);
    }

    #endregion

    #region Movement functions
    public virtual Vector3 GetInitialNextDestinationBlock(MovementDirection dir)
    {
        Vector3 nextPos = transform.position;

        switch (dir)
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
        return nextPos;
    }

    //any entity specific updates, like the player look direction can edit the final direction being moved
    public virtual MovementDirection GetFinalMovementDirection(MovementDirection dir)
    {
        return dir;
    }

    public virtual void GetProjectedDestinationBlockPosition(MovementDirection dir)
    {
        Vector3 nextPos = GetInitialNextDestinationBlock(dir);
        

        DownDirection downDir = GetCurrentDownDirection();

        //remove height to get logical center of current block
        switch (downDir)
        {
            case DownDirection.Ydown:
                nextPos.y = Mathf.Floor(nextPos.y + 0.01f);
                break;
            case DownDirection.Yup:
                nextPos.y = Mathf.Ceil(nextPos.y - 0.01f);
                break;
            case DownDirection.Xleft:
                nextPos.x = Mathf.Floor(nextPos.x + 0.01f);
                break;
            case DownDirection.Xright:
                nextPos.x = Mathf.Ceil(nextPos.x - 0.01f);
                break;
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z + 0.01f);
                break;
            case DownDirection.Zforward:
                nextPos.z = Mathf.Ceil(nextPos.z - 0.01f);
                break;
        }
        //Debug.Log(nextPos);

        //if in a partial block find exit height (if height >= 1 then the player has gone up a level and we'll check for collision there)
        Block currentBlock = map.GetCurrentlyOccupiedBlock(transform.position, GetCurrentDownDirection());
        float exitHeight = currentBlock != null ? currentBlock.CalculateAttemptedExitEdgeHeight(nextPos, transform.up, GetCurrentDownDirection()) : 0;
        nextPos += transform.up * exitHeight;
        switch (downDir)
        {
            case DownDirection.Ydown:
                nextPos.y = Mathf.Floor(nextPos.y + 0.01f);
                break;
            case DownDirection.Yup:
                nextPos.y = Mathf.Ceil(nextPos.y - 0.01f);
                break;
            case DownDirection.Xleft:
                nextPos.x = Mathf.Floor(nextPos.x + 0.01f);
                break;
            case DownDirection.Xright:
                nextPos.x = Mathf.Ceil(nextPos.x - 0.01f);
                break;
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z + 0.01f);
                break;
            case DownDirection.Zforward:
                nextPos.z = Mathf.Ceil(nextPos.z - 0.01f);
                break;
        }

        //check for stairs going down
        Block straightForwardDestBlock = map.GetBlock(nextPos);
        //if straight forwad block is not ground in itself need to check if below block is a staircase for smooth movement
        if (straightForwardDestBlock == null || !straightForwardDestBlock.blocksAllMovement && !straightForwardDestBlock.isGround)
        {
            Vector3 belowDest = nextPos - transform.up;

            Block belowDestBlock = map.GetBlock(belowDest);
            if (belowDestBlock != null && belowDestBlock.isGround && belowDestBlock.canEntityEnter(this))
            {
                nextPos = belowDest;
                nextPos += transform.up * belowDestBlock.GetMidBlockHeight(-transform.up);
            }
        }
        //block exists and is full (player will try to climb ontop of it if possible)
        else if (straightForwardDestBlock != null && exitHeight < 1.0f)
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
                nextPos.y = Mathf.Floor(nextPos.y + 0.01f);
                break;
            case DownDirection.Yup:
                nextPos.y = Mathf.Ceil(nextPos.y - 0.01f);
                break;
            case DownDirection.Xleft:
                nextPos.x = Mathf.Floor(nextPos.x + 0.01f);
                break;
            case DownDirection.Xright:
                nextPos.x = Mathf.Ceil(nextPos.x - 0.01f);
                break;
            case DownDirection.Zback:
                nextPos.z = Mathf.Floor(nextPos.z + 0.01f);
                break;
            case DownDirection.Zforward:
                nextPos.z = Mathf.Ceil(nextPos.z - 0.01f);
                break;
        }
        projectedDestinationBlock = Map.GetIntVector3(nextPos);
    }

    public virtual bool IsDestinationOccupied(Vector3Int destinationToCheck)
    {
        //Debug.Log("current: " + GetCurrentBlockPosition() + " dest: " + destinationToCheck);
        //block collision
        Block destinationBlock = map.GetBlock(destinationToCheck);
        if (destinationBlock != null)
        {
            if (!destinationBlock.canEntityEnter(this)) { return true; }
            if (destinationBlock.GetMidBlockHeight(-transform.up) > 0.25)
            {
                Vector3 headCheckPos = destinationToCheck;
                headCheckPos += transform.up;
                Block headCheckBlock = map.GetBlock(Map.GetIntVector3(headCheckPos));
                if (headCheckBlock != null) { return true; }
            }
        }

        //check for entities
        FullGridMoveable blockingFGM = gameController.GetFGMAtPosition(destinationToCheck);
        if (blockingFGM != null) { return true; }
        return false;
    }

    public bool isEntityGrounded()
    {
        DownDirection downDir = GetCurrentDownDirection();

        float downDirectionEntityHeight = Block.GetPositionsDownOrientedHeight(transform.position, downDir);

        Block currentBlock = map.GetCurrentlyOccupiedBlock(transform.position, GetCurrentDownDirection());
        if (currentBlock != null && currentBlock.isGround)
        {
            float blockDownDirectionHeight = Block.GetPositionsDownOrientedHeight(currentBlock.transform.position, downDir);

            //In block but floating above it and need to fall
            if (downDirectionEntityHeight - blockDownDirectionHeight - currentBlock.GetMidBlockHeight(-transform.up) > 0.01f) { /*Debug.Log("floating in block");*/ return false; }
            return true;
        }
        //floating in empty block
        else if (currentBlock == null)
        {
            switch (downDir)
            {
                case DownDirection.Ydown:
                case DownDirection.Xleft:
                case DownDirection.Zback:
                    if (downDirectionEntityHeight > Mathf.Floor(downDirectionEntityHeight)) { /*Debug.Log("falling floor");*/ return false; }
                    break;

                case DownDirection.Yup:
                case DownDirection.Xright:
                case DownDirection.Zforward:
                    if (downDirectionEntityHeight < Mathf.Ceil(downDirectionEntityHeight)) { /*Debug.Log("falling ceil");*/ return false; }
                    break;
            }
        }

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

        if (groundBlock == null || !groundBlock.isGround || groundBlock.GetMidBlockHeight(-transform.up) < 0.99f) { /*Debug.Log("fall no block below");*/ return false; }

        return true;
    }

    #endregion

    #region Helper Functions

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

    #endregion

    #region Commands
    //Active decisions by the entity such as to move or attack
    public virtual Command GetCommand()
    {
        return stateMachine.currentState.StateGetCommand();
    }

    //commands that arise from the enemies current environment (sliding on ice, or falling in a hole for example)
    public virtual Command GetPassiveCommand()
    {
        Command cmd = null;

        //check for falling
        if (affectedByGravity)
        {
            if (!isEntityGrounded())
            {
                cmd = new FallCommand(this, transform.position);
            }
        }

        return cmd;
    }

    public virtual List<Command> GetCommands() { return null; }

    public virtual List<Command> GetPassiveCommands() { return null; }

    #endregion
}
