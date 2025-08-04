using Codice.ThemeImages;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : MonoBehaviour
{
    //the wall is is positioned/oriented between these two points on the grid
    Vector3Int positionA, positionB;

    [SerializeField]
    public bool blocksMovement;

    public void SetPositions(Vector3Int a, Vector3Int b)
    {
        positionA = a;
        positionB = b;
    }

    // Start is called before the first frame update
    public void BlockMovement()
    {
        blocksMovement = true;
    }

    public void UnblockMovement()
    {
        blocksMovement = false;
    }

    public bool DoesMovementPassThroughWall(Vector3Int src, Vector3Int dest)
    {
        if (src == positionA)
        {
            return dest == positionB;
        }
        else if (src == positionB)
        {
            return dest == positionA;
        }
        return false;
    }
}
