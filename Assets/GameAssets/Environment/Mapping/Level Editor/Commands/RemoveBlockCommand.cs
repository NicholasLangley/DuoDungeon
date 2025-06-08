using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBlockCommand : Command
{
    Vector3Int placementLocation;
    string listID, baseID, varientID;
    Quaternion blockRotation;
    ObjectPlacer objectPlacer;

    public RemoveBlockCommand(ObjectPlacer placer, Vector3Int destination, string list_ID, string base_ID, string varient_ID, Quaternion rotation)
    {
        placementLocation = destination;

        listID = list_ID;
        baseID = base_ID;
        varientID = varient_ID;
        blockRotation = rotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.RemoveBlock(placementLocation);
    }

    public override void Undo()
    {
        objectPlacer.PlaceBlock(placementLocation, blockRotation, listID, baseID, varientID);
    }
}
