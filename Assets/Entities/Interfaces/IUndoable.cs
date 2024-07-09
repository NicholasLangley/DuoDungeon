using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUndoable
{
    public bool currentlyUndoing { get; set; }
}
