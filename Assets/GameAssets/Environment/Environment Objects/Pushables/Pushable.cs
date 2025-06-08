using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : FullGridMoveable
{
    public bool toBePushedThisRound;
    public MovementDirection pushDirection;
    public ComplexBlock complexBlock;
    protected override void Awake()
    {
        complexBlock = gameObject.GetComponent<ComplexBlock>();

        #region Create State Machine and states
        stateMachine = new StateMachine();

        idleState = new FGM_ComplexBlock_IdleState(this, stateMachine, complexBlock);
        movementState = new FGM_MovementState(this, stateMachine);
        movementBlockedState = new FGM_MovementBlockedState(this, stateMachine);
        rotationState = new FGM_RotationState(this, stateMachine);
        fallingState = new FGM_FallingState(this, stateMachine);
        //unique entities may override this
        #endregion
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        toBePushedThisRound = false;
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    #region Commands
    //Active decisions by the entity such as to move or attack
    public override Command GetCommand() 
    {
        Command cmd = null;

        if (toBePushedThisRound)
        {
            cmd = new MoveCommand(this, pushDirection);
        }

        toBePushedThisRound = false;
        return cmd; 
    }

    //commands that arise from the current environment (sliding on ice, or falling in a hole for example)
    public override Command GetPassiveCommand()
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

     #endregion

    public bool AttemptPush(Vector3 pushDirectionVector, Block pushedBlockComponent)
    {
        pushDirection = getLocalPushDirection(pushDirectionVector);

        foreach (Vector3Int subBlockPos in complexBlock.getSubBlockPositions(Map.GetIntVector3(transform.position)))
        {
            if (IsDestinationOccupied(CalculateProjectedDestinationBlockForPosition(pushDirection, subBlockPos))) {return false; }
        }

        toBePushedThisRound = true;
        return true;
 
    }

    MovementDirection getLocalPushDirection(Vector3 pushDirectionVector)
    {
        if(pushDirectionVector == transform.forward)
        {
            return MovementDirection.FORWARD;
        }
        else if (pushDirectionVector == -transform.forward)
        {
            return MovementDirection.BACKWARD;
        }
        else if (pushDirectionVector == transform.right)
        {
            return MovementDirection.RIGHT;
        }
        else
        {
            return MovementDirection.LEFT;
        }
    }

    #region FGM Overrides

    public override void GetProjectedDestinationBlockPosition(MovementDirection dir)
    {
        //finds the sub blocks that ends up moving the most in the positive up direction (based on gravity)
        //Those blocks are the ones that collided soonest with the ground and are the resting points of the block.
        //This max transformation vector is applied tomain position of the block to get the final grid pos
        //Need a custom fgm_movement state to handle proper mvement calculations though as the current one only takes mainposition block into account
        Vector3Int maxHeightSubBlockTransformationVector = Vector3Int.zero;
        float maxheightDotProductProjection = -999f;
        foreach (Vector3Int subBlockPos in complexBlock.getSubBlockPositions(Map.GetIntVector3(transform.position)))
        {

            Vector3Int destPos = CalculateProjectedDestinationBlockForPosition(movementDirection, subBlockPos);
            Vector3Int transformationVector = destPos - subBlockPos;

            float projectedHeight = Vector3.Dot(transformationVector, -gravityDirection);
            if (projectedHeight > maxheightDotProductProjection) 
            { 
                maxHeightSubBlockTransformationVector = transformationVector; 
                maxheightDotProductProjection = projectedHeight;
            }
        }

        projectedDestinationBlock = Map.GetIntVector3(transform.position) + maxHeightSubBlockTransformationVector;
    }

    #endregion
}
