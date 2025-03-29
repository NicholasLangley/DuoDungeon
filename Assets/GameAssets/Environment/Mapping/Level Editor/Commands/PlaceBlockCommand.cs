using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlockCommand : Command
{
    Vector3Int placementLocation;
    string newListID, newBaseID, newVarientID, originalListID, originalBaseID, originalVarientID;
    Quaternion newBlockRotation, originalBlockRotation;
    ObjectPlacer objectPlacer;

    public PlaceBlockCommand(ObjectPlacer placer, Vector3Int destination, string listID, string baseID, string varientID, Quaternion newRotation, string oldListID = "DELETE", string oldBaseID = "DELETE", string oldVarientID = "DELETE", Quaternion oldRotation = new Quaternion())
    {
        placementLocation = destination;

        newListID = listID;
        newBaseID = baseID;
        newVarientID = varientID;
        newBlockRotation = newRotation;

        originalListID = oldListID;
        originalBaseID = oldBaseID;
        originalVarientID = oldVarientID;
        originalBlockRotation = oldRotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.PlaceBlock(placementLocation, newBlockRotation, newListID, newBaseID, newVarientID);
    }

    public override void Undo()
    {
        objectPlacer.PlaceBlock(placementLocation, originalBlockRotation, originalListID, originalBaseID, originalVarientID);
    }
}
