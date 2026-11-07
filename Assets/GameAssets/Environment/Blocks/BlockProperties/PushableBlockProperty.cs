using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PushableBlockProperty", menuName = "Blocks/BlockProperties/PushableBlockProperty")]
public class PushableBlockProperty : BlockProperty
{
    //TODO functions for accessing / updating logical grid blocks 
    //steal from old complex blocks class

    public static bool IsPushable(BlockData blockData)
    {
        foreach (BlockProperty property in blockData.properties)
        {
            if (property is PushableBlockProperty)
            {
                return true;
            }
        }

        return false;
    }
}