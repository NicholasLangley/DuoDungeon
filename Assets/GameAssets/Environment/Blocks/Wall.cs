using Codice.ThemeImages;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : Placeable
{
    //how tall the wall is from each orientation
    float heightFromBottom, heightFromTop, heightFromLeft, heightFromRight;
    
    //how thick the wall is (for landing on)
    private float wallThickness;

    [SerializeField]
    public bool blocksMovement;

    public void SetHeights(float bot, float top, float left, float right)
    {
        heightFromBottom = bot;
        heightFromTop = top;
		heightFromLeft = left;
		heightFromRight = right;
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
}
