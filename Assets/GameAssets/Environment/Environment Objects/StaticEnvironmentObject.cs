using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

//Any environment object that is not expected to move from grid space to grid space
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Commands
    //Active decisions by the entity such as to move or attack
    public abstract Command GetCommand();

    //commands that arise from the enemies current environment (sliding on ice, or falling in a hole for example)
    public abstract Command GetPassiveCommand();

    public List<Command> GetCommands() { return null; }
    public List<Command> GetPassiveCommands() { return null; }

    #endregion
}
