using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to group commands together when running them all at once
//Mostly used for the map editor placing multiple blocks at once so they can be undone in a single action
public class CommandGroup : Command
{
    List<Command> commands;

public CommandGroup()
{
    commands = new List<Command>();
}

public void AddCommand(Command command)
{
    commands.Add(command);
}

//I don't foresee this being used but who knows
    public void RemoveCommand(int index)
    {
        if (index >= 0 && index < commands.Count)
        {
            commands.RemoveAt(index);
        }
    }

public override void Execute()
{
    if (commands.Count > 0)
    {
        foreach (Command command in commands)
        {
            command.Execute();
        }
    }
}

public override void Undo()
{
    if (commands.Count > 0)
    {
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            commands[i].Undo();
        }
    }
}
}