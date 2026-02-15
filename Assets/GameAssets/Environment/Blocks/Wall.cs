using Codice.ThemeImages;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : Placeable
{
    //How far the wall extends out from the center line (total width is 2x thickness)
    [SerializeField]
    public float thickness;
    
    [SerializeField]
    public bool blocksMovement;

    // Start is called before the first frame update
    public void BlockMovement()
    {
        blocksMovement = true;
    }

    public void UnblockMovement()
    {
        blocksMovement = false;
    }
}
