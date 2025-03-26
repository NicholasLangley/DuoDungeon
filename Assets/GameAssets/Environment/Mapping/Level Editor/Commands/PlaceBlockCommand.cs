using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlockCommand : Command
{
    Vector3Int placementLocation;
    string newBaseID, newVarientID, originalBaseID, originalVarientID;
    Quaternion newBlockRotation, originalBlockRotation;
    ObjectPlacer objectPlacer;

    public PlaceBlockCommand(ObjectPlacer placer, Vector3Int destination, string baseID, string varientID, Quaternion newRotation, string oldBaseID = "DELETE", string oldVarientID = "DELETE", Quaternion oldRotation = new Quaternion())
    {
        placementLocation = destination;

        newBaseID = baseID;
        newVarientID = varientID;
        newBlockRotation = newRotation;

        originalBaseID = oldBaseID;
        originalVarientID = oldVarientID;
        originalBlockRotation = oldRotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.PlaceBlock(placementLocation, newBlockRotation, newBaseID, newVarientID);
    }

    public override void Undo()
    {
        objectPlacer.PlaceBlock(placementLocation, originalBlockRotation, originalBaseID, originalVarientID);
    }
}
