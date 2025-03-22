using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : StaticEnvironmentObject
{
    bool cracked;
    FullGridMoveable previousStepper;
    //multiple directions simultaneously?

    #region Commands
    //Active decisions by the object such as to move or attack
    public override Command GetCommand()
    {
        return null;
    }

    //commands that arise from the objects current environment (sensing an entity for example)
    public override Command GetPassiveCommand()
    {
        return null;
    }

    #endregion
}
