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
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            return AttemptMovement();
        }
        

        

        return null;
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

        Vector3 nextRedPos = redPlayer.transform.position;
        Vector3 nextBluePos = bluePlayer.transform.position;

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            nextRedPos += redPlayer.transform.forward;
            nextBluePos += bluePlayer.transform.forward;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            nextRedPos -= redPlayer.transform.forward;
            nextBluePos -= bluePlayer.transform.forward;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            nextRedPos += redPlayer.transform.right;
            nextBluePos += bluePlayer.transform.right;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            nextRedPos -= redPlayer.transform.right;
            nextBluePos -= bluePlayer.transform.right;
        }
        else { return null; }

        MoveCommand mcRed = new MoveCommand(redPlayer, redPlayer.transform.position, nextRedPos);
        MoveCommand mcBlue = new MoveCommand(bluePlayer, bluePlayer.transform.position, nextBluePos);

        commands.Add(mcRed);
        commands.Add(mcBlue);

        return commands;
    }
}
