using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//complex block means athe block may move/ become misaligned from the grid.
//This means any logical grid block(s) need to be computed whenever the "transform" is updated
[CreateAssetMenu(fileName = "ComplexBlockProperty", menuName = "Blocks/BlockProperties/ComplexBlockProperty")]
public class ComplexBlockProperty : BlockProperty
{
    //TODO functions for accessing / updating logical grid blocks 
    //steal from old complex blocks class
}

public struct AdditionalMultiBlock
{
    [SerializeField] public Vector3 offset;
    [SerializeField] public BlockSideDefinitions sides;
}

//denotes blocks that take up more than one space
//main block is still stored in the blockData
//additional blocks  are stored here and contain a Vector3 offset for how far from the main block they are
//each additional block is
[CreateAssetMenu(fileName = "MultiBlockProperty", menuName = "Blocks/BlockProperties/MultiBlockProperty")]
public class MultiBlockProperty : BlockProperty
{
    [SerializeField] public List<AdditionalMultiBlock> additionalBlocks;
    
    //TODO maybe add / remove blocks? could use a dict by offset instead?
}