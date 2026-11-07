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