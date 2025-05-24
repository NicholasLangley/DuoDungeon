using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePlayerCommand : Command
{
    Vector3Int placementLocation, originalPlayerPosition;
    Quaternion playerRotation, originalPlayerRotation;
    bool isRedPlayer;
    ObjectPlacer objectPlacer;

    public PlacePlayerCommand(ObjectPlacer placer, Vector3Int destination, Quaternion rotation, bool isRed, Vector3Int oldPlayerPosition, Quaternion oldPlayerRotation)
    {
        placementLocation = destination;

        playerRotation = rotation;
        isRedPlayer = isRed;

        originalPlayerPosition = oldPlayerPosition;
        originalPlayerRotation = oldPlayerRotation;

        objectPlacer = placer;
    }

    public override void Execute()
    {
        objectPlacer.PlacePlayer(placementLocation, playerRotation, isRedPlayer);
    }

    public override void Undo()
    {
        objectPlacer.PlacePlayer(originalPlayerPosition, originalPlayerRotation, isRedPlayer);
    }
}
