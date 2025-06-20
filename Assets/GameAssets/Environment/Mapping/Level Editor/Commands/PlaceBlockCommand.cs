using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlockCommand : Command
{
    Vector3Int placementLocation;
    string listID, baseID, varientID;
    Quaternion blockRotation;
    ObjectPlacer objectPlacer;

    public PlaceBlockCommand(ObjectPlacer placer, Vector3Int destination, string list_ID, string base_ID, string varient_ID, Quaternion rotation)
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
        objectPlacer.PlaceBlock(placementLocation, blockRotation, listID, baseID, varientID);
    }

    public override void Undo()
    {
        objectPlacer.RemoveBlock(placementLocation);
    }
}
