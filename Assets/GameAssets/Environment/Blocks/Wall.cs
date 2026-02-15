using Codice.ThemeImages;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : Placeable
{
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
