using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandable
{
    //all commandables must be free before next next turn/undo can occur
    //maybe rethink this if chains of events become too complex
    bool busy { get; set; }

    //the logic each entity does each turn, returns null if no action is taken
    public Command GetCommand();

    public List<Command> GetCommands();

}
