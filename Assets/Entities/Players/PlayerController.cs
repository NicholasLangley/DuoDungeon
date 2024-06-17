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
        
        List<Command> commands = new List<Command>();

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveCommand mcRed = new MoveCommand(redPlayer, redPlayer.transform.position, redPlayer.transform.position + redPlayer.transform.forward);
            MoveCommand mcBlue = new MoveCommand(bluePlayer, bluePlayer.transform.position, bluePlayer.transform.position + bluePlayer.transform.forward);

            commands.Add(mcRed);
            commands.Add(mcBlue);

            return commands;
        }

        return null;
    }

    public Command GetCommand() { return null; }

    public bool CheckIfBusy()
    {
        return (redPlayer.busy || bluePlayer.busy);
    }
}
