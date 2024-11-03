using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlockCommand : Command
{
    Vector3Int placementLocation;
    int newBlockID, originalBlockID;
    Quaternion newBlockRotation, originalBlockRotation;
    ObjectPlacer objectPlacer;

    public PlaceBlockCommand(ObjectPlacer placer, Vector3Int destination, int blockID, Quaternion newRotation, int oldBlockID = -1, Quaternion oldRotation = new Quaternion())
    {
        placementLocation = destination;

        newBlockID = blockID;
        newBlockRotation = newRotation;

        originalBlockID = oldBlockID;
        originalBlockRotation = oldRotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.PlaceBlock(placementLocation, newBlockRotation, newBlockID);
    }

    public override void Undo()
    {
        objectPlacer.PlaceBlock(placementLocation, originalBlockRotation, originalBlockID);
    }
}
