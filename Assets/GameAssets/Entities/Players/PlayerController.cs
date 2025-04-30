using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : ICommandable
{
    PlayerEntity redPlayer;
    PlayerEntity bluePlayer;
    PlayerInputHandler playerInputHandler;
    GameController gameController;

    public bool busy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public PlayerController(PlayerEntity red, PlayerEntity blue, PlayerInputHandler inputHandler, GameController gc)
    {
        redPlayer = red;
        redPlayer.gameController = gc;

        bluePlayer = blue;
        bluePlayer.gameController = gc;

        playerInputHandler = inputHandler;
        gameController = gc;
    }

    public void SpawnPlayers(Map map)
    {
        //reset player stats if any

        redPlayer.transform.position = map.redPlayerSpawn;
        redPlayer.transform.rotation = map.redPlayerSpawnRotation;
        redPlayer.map = map;

        bluePlayer.transform.position = map.bluePlayerSpawn;
        bluePlayer.transform.rotation = map.bluePlayerSpawnRotation;
        bluePlayer.map = map;
    }

    public void SetPlayerGravity()
    {
        redPlayer.setGravityDirection(-redPlayer.transform.up);
        bluePlayer.setGravityDirection(-bluePlayer.transform.up);
    }

    public List<Command> GetCommands()
    {
        //Movement
        if (playerInputHandler.moveInput != Vector2.zero)
        {
            return AttemptMovement();
        }

        //rotation
        else if (playerInputHandler.rotateInput < 0 && redPlayer.getClassicDungeonCrawlerMode()) { return RotatePlayers(-90f); }
        else if (playerInputHandler.rotateInput > 0 && redPlayer.getClassicDungeonCrawlerMode()) { return RotatePlayers(90f); }

        //Attack
        else if (playerInputHandler.attackInput) { return AttackWithPlayers(); }

        //Interact TODO
        else if (playerInputHandler.interactInput) { }

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
    public Command GetPassiveCommand() { return null; }

    public bool CheckIfBusy()
    {
        return (redPlayer.busy || bluePlayer.busy);
    }

    //movement
    List<Command> AttemptMovement()
    {
        List<Command> commands = new List<Command>();

        MovementDirection initialMovementDir;

        //get next position for each player
        if (playerInputHandler.moveInput.y > 0)
        {
            initialMovementDir = MovementDirection.FORWARD;
        }
        else if (playerInputHandler.moveInput.y < 0)
        {
            initialMovementDir = MovementDirection.BACKWARD;
        }
        else if (playerInputHandler.moveInput.x > 0)
        {
            initialMovementDir = MovementDirection.RIGHT;
        }
        else if (playerInputHandler.moveInput.x < 0)
        {
            initialMovementDir = MovementDirection.LEFT;
        }
        else { return null; }

        MovementDirection redMovementDir = redPlayer.GetFinalMovementDirection(initialMovementDir);
        MovementDirection blueMovementDir = bluePlayer.GetFinalMovementDirection(initialMovementDir);


        redPlayer.GetProjectedDestinationBlockPosition(redMovementDir);
        bluePlayer.GetProjectedDestinationBlockPosition(blueMovementDir);
        Vector3Int nextRedPos = redPlayer.projectedDestinationBlock;
        Vector3Int nextBluePos = bluePlayer.projectedDestinationBlock;

        //check if space is occupied
        bool redBlocked = redPlayer.IsDestinationOccupied(nextRedPos);
        bool blueBlocked = bluePlayer.IsDestinationOccupied(nextBluePos);

        //check if blocking object is a pushable object
        if (redBlocked)
        {
            FullGridMoveable blockingFGM = gameController.GetFGMAtPosition(nextRedPos);
            if (blockingFGM != null && blockingFGM is Pushable) 
            {
                Pushable pushable = blockingFGM as Pushable;
                bool pushing = pushable.AttemptPush(redPlayer.transform);
                if (pushing) { redBlocked = false; }
            }
        }
        if(blueBlocked)
        {
            FullGridMoveable blockingFGM = gameController.GetFGMAtPosition(nextBluePos);
            if (blockingFGM != null && blockingFGM is Pushable)
            {
                Pushable pushable = blockingFGM as Pushable;
                bool pushing = pushable.AttemptPush(bluePlayer.transform);
                if (pushing) { blueBlocked = false; }
            }
        }

        //allow blocked player to move IFF it is moving into a space occupied by the other player which is vacating it
        if (redBlocked && !blueBlocked && nextRedPos == bluePlayer.GetCurrentBlockPosition())
        {
            redBlocked = false;
        }
        else if (blueBlocked && !redBlocked && nextBluePos == redPlayer.GetCurrentBlockPosition())
        {
            blueBlocked = false;
        }

        //red player takes precedence if both try to move to same location
        if (nextRedPos == nextBluePos) { blueBlocked = true; }

        Command cmdRed;
        Command cmdBlue;

        //return command based on if space is occupied or not
        cmdRed = redBlocked ? new MovementBlockedCommand(redPlayer, redMovementDir) : new MoveCommand(redPlayer, redMovementDir);
        cmdBlue = blueBlocked ? new MovementBlockedCommand(bluePlayer, blueMovementDir) : new MoveCommand(bluePlayer, blueMovementDir);

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

    List<Command> AttackWithPlayers()
    {
        List<Command> commands = new List<Command>();

        Command cmdRed;
        Command cmdBlue;

        //return command based on if space is occupied or not
        cmdRed = new AttackCommand(redPlayer);
        cmdBlue = new AttackCommand(bluePlayer);

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

    public PlayerEntity GetPlayerAtPosition(Vector3Int destinationToCheck)
    {
        if (redPlayer.GetCurrentBlockPosition() == destinationToCheck) { return redPlayer; }
        if (bluePlayer.GetCurrentBlockPosition() == destinationToCheck) { return bluePlayer; }
        return null;
    }

    #region camera settings
    public void updatePlayerAim()
    {
        redPlayer.UpdateCameraAim(playerInputHandler.aimInput);
        bluePlayer.UpdateCameraAim(playerInputHandler.aimInput);
    }

    public void setMouseSensitivity(Vector2 sense)
    {
        redPlayer.setMouseSensitivity(sense);
        bluePlayer.setMouseSensitivity(sense);
    }

    public void setMouseInversion(bool xInvert, bool yInvert)
    {
        redPlayer.setMouseInversion(xInvert, yInvert);
        bluePlayer.setMouseInversion(xInvert, yInvert);
    }

    public void setClassicDungeonCrawlerMode(bool modeSetting)
    {
        redPlayer.setClassicDungeonCrawlerMode(modeSetting);
        bluePlayer.setClassicDungeonCrawlerMode(modeSetting);
    }

    #endregion
}
