using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockListScriptableObject", menuName = "ScriptableObjects/BlockList")]
public class BlockList : ScriptableObject
{
    [SerializeField]
    List<GameObject> blocks;

    public GameObject getBlock(int blockId)
    {
        return blocks[blockId];
    }

    public int GetLength()
    {
        return blocks.Count;
    }
}
