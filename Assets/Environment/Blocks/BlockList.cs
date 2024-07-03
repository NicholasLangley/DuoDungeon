using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockListScriptableObject", menuName = "ScriptableObjects/BlockList")]
public class BlockList : ScriptableObject
{
    [SerializeField]
    List<Block> blocks;

    public Block getBlock(int blockId)
    {
        return blocks[blockId];
    }
}
