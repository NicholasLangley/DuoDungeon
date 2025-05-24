using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    //2DList to handle undoing events that occured sequentially in the same turn (ie, being pushed onto a tile with a trap)
    public List<List<Command>> TurnActions;

    public Turn()
    {
        TurnActions = new List<List<Command>>();
        AddTurnLayer();
    }

    public void AddTurnLayer()
    {
        TurnActions.Add(new List<Command>());
    }

    public void AddCommand(Command cmd)
    {
        TurnActions[TurnActions.Count - 1].Add(cmd);
    }

    public void undoLayer(int i)
    {
        for(int j = TurnActions[i].Count - 1; j >= 0; j--)
        {
            TurnActions[i][j].Undo();
        }
    }

    public void redoLayer(int i)
    {
        for (int j = 0; j < TurnActions[i].Count; j++)
        {
            TurnActions[i][j].Execute();
        }
    }
}
