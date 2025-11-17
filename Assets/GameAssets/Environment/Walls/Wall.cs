using Codice.ThemeImages;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : MonoBehaviour
{
    //how tall the wall is from each orientation
    float heightFromBottom, heightFromTop, heightFromLeft, heightFromRight;
    
    //how thick the wall is (for landing on)
    private float wallThickness;

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
