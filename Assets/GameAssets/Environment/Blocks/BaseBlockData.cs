using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//contains the base info shared between all blocks of a specific shape
[CreateAssetMenu(fileName = "baseBlockData", menuName = "Blocks/BaseBlockData")]
public class BaseBlockData : ScriptableObject
{
    [SerializeField]    
    public BlockSideDefinitions blockSides;
}

