using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//any environment object that doesn't need in depth movement
//If the object is going to be moving on the full grid frrequently such as a pushable, use the FullGridMoveable class
public abstract class StaticEnvironmentObject : MonoBehaviour, ICommandable, IUndoable
{
    #region ICommandable variables
    //ICommandable variables
    [field: SerializeField] public bool busy { get; set; }

    #endregion

    #region IUndoable Variables
    //IUndoable
    public bool currentlyUndoing { get; set; }

    #endregion

    #region Commands
    //Active decisions by the object such as to move or attack
    public abstract Command GetCommand();

    //commands that arise from the objects current environment (sensing an entity for example)
    public abstract Command GetPassiveCommand();

    public List<Command> GetCommands() { return null; }
    public List<Command> GetPassiveCommands() { return null; }

    #endregion
}
