using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePlayerCommand : Command
{
    Vector3Int placementLocation, originalPlayerPosition;
    Quaternion playerRotation, originalPlayerRotation, originalBlockRotation;
    string originalListID, originalBaseID, originalVarientID;
    bool isRedPlayer;
    ObjectPlacer objectPlacer;

    public PlacePlayerCommand(ObjectPlacer placer, Vector3Int destination, Quaternion rotation, bool isRed, Vector3Int oldPlayerPosition, Quaternion oldPlayerRotation, string oldListID = "DELETE", string oldBaseID = "DELETE", string oldVarientID = "DELETE", Quaternion oldRotation = new Quaternion())
    {
        placementLocation = destination;

        playerRotation = rotation;
        isRedPlayer = isRed;

        originalPlayerPosition = oldPlayerPosition;
        originalPlayerRotation = oldPlayerRotation;

        originalListID = oldListID;
        originalBaseID = oldBaseID;
        originalVarientID = oldVarientID;
        originalBlockRotation = oldRotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.PlacePlayer(placementLocation, playerRotation, isRedPlayer);
    }

    public override void Undo()
    {
        objectPlacer.PlacePlayer(originalPlayerPosition, originalPlayerRotation, isRedPlayer);
        if (string.Compare(originalBaseID, "DELETE") != 0) { objectPlacer.PlaceBlock(placementLocation, originalBlockRotation, originalListID, originalBaseID, originalVarientID); }
    }
}
