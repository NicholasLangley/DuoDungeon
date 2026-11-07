using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // <-- THIS IS THE MISSING PIECE
public struct AdditionalMultiBlock
{
    // Note: Since these are public, [SerializeField] is optional, 
    // but keeping it doesn't hurt.
    [SerializeField] public Vector3 offset;
    [SerializeField] public BlockSideDefinitions sides;
}

[CreateAssetMenu(fileName = "MultiBlockProperty", menuName = "Blocks/BlockProperties/MultiBlockProperty")]
public class MultiBlockProperty : BlockProperty
{
    [SerializeField] public List<AdditionalMultiBlock> additionalBlocks;
}