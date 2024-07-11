using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICommandable
{
    [SerializeField]
    PlayerEntity redPlayer;
    [SerializeField]
    PlayerEntity bluePlayer;
    public bool busy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Command> GetCommands()
    {
        //Movement
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            return AttemptMovement();
        }

        //rotation
        else if (Input.GetKeyDown(KeyCode.Q)) { return RotatePlayers(-90f); }
        else if (Input.GetKeyDown(KeyCode.E)) { return RotatePlayers(90f); }

        //Attack


        //Interact




        return null;
    }

    public List<Command> GetPassiveCommands()
    {
        List<Command> foundCommands = new List<Command>();

        Command redCmd = redPlayer.GetPassiveCommand();
        Command blueCmd = bluePlayer.GetPassiveCommand();

        if (redCmd != null) { foundCommands.Add(redCmd); }
        if (blueCmd != null) { foundCommands.Add(blueCmd); }

        return foundCommands;
    }

    public Command GetCommand() { return null; }

    public bool CheckIfBusy()
    {
        return (redPlayer.busy || bluePlayer.busy);
    }

    //movement
    List<Command> AttemptMovement()
    {
        List<Command> commands = new List<Command>();

        MovementDirection movementDir;

        //get next position for each player
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            movementDir = MovementDirection.FORWARD;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            movementDir = MovementDirection.BACKWARD;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            movementDir = MovementDirection.RIGHT;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            movementDir = MovementDirection.LEFT;
        }
        else { return null; }

        Vector3 nextRedPos = redPlayer.GetProjectedDestinationPosition(movementDir);
        Vector3 nextBluePos = bluePlayer.GetProjectedDestinationPosition(movementDir);

        //check if space is occupied
        bool redBlocked = redPlayer.IsDestinationOccupied(nextRedPos);
        bool blueBlocked = bluePlayer.IsDestinationOccupied(nextBluePos);

        //allow blocked player to move IFF it is moving into a space occupied by the other player which is vacating it
        if(redBlocked && !blueBlocked && nextRedPos == bluePlayer.transform.position)
        {
            redBlocked = false;
        }
        else if (blueBlocked && !redBlocked && nextBluePos == redPlayer.transform.position)
        {
            blueBlocked = false;
        }

        //red player takes precedence if both try to move to same location
        if (nextRedPos == nextBluePos) { blueBlocked = true; }

        Command cmdRed;
        Command cmdBlue;

        //return command based on if space is occupied or not
        cmdRed = redBlocked ? new MovementBlockedCommand(redPlayer, movementDir) : new MoveCommand(redPlayer, movementDir);
        cmdBlue = blueBlocked ? new MovementBlockedCommand(bluePlayer, movementDir) : new MoveCommand(bluePlayer, movementDir);

        commands.Add(cmdRed);
        commands.Add(cmdBlue);

        return commands;
    }

    List<Command> RotatePlayers(float degrees)
    {
        List<Command> commands = new List<Command>();

        Command cmdRed;
        Command cmdBlue;

        //return command based on if space is occupied or not
        cmdRed = new RotationCommand(redPlayer, degrees);
        cmdBlue = new RotationCommand(bluePlayer, degrees);

        commands.Add(cmdRed);
        commands.Add(cmdBlue);

        return commands;
    }

    public void SetPlayersToUndo()
    {
        redPlayer.currentlyUndoing = true;
        bluePlayer.currentlyUndoing = true;
    }
    public void UnsetPlayersFromUndo()
    {
        redPlayer.currentlyUndoing = false;
        bluePlayer.currentlyUndoing = false;
    }
}
