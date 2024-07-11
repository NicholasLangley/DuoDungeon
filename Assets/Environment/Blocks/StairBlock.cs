using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBlock : Block
{
    [SerializeField] public float stairHeight;

    // Start is called before the first frame update
    void Awake()
    {
        blockType = BLOCK_TYPE.STAIR;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
