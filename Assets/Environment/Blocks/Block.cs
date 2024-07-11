using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCK_TYPE { DEFAULT, STAIR }

public class Block : MonoBehaviour
{
    public int blockID { get; set; }

    [SerializeField]
    public bool blocksMovement, blocksProjectiles, isInbetweenBlock;

    protected BLOCK_TYPE blockType;

    // Start is called before the first frame update
    void Awake()
    {
        blockType = BLOCK_TYPE.DEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {

    }

    public BLOCK_TYPE GetBlockType()
    {
        return blockType;
    }


}
