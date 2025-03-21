using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : FullGridMoveable
{
    public bool toBePushedThisRound;
    public MovementDirection pushDirection;
    protected override void Awake()
    {
        #region Create State Machine and states
        stateMachine = new StateMachine();

        idleState = new FGM_IdleState(this, stateMachine);
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

    public bool AttemptPush(Transform pusher)
    {
        if(true)
        {
            toBePushedThisRound = true;
            pushDirection = getLocalPushDirection(pusher);
            return true;
        }

        return false;
    }

    MovementDirection getLocalPushDirection(Transform pusher)
    {
        Vector3 pushVector = transform.position - pusher.position;

        if(pushVector == transform.forward)
        {
            return MovementDirection.FORWARD;
        }
        else if (pushVector == -transform.forward)
        {
            return MovementDirection.BACKWARD;
        }
        else if (pushVector == transform.right)
        {
            return MovementDirection.RIGHT;
        }
        else
        {
            return MovementDirection.LEFT;
        }
    }
}
