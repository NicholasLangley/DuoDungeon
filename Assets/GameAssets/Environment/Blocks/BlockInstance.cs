using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a particular instance of a block containing its blockFlyweightData, and its  rotation
//position is used as the map key so doesn't need to be stored here
//index is the specific index of the transform in the FlyweightRenderer list. used for disabling / updating the transform if needed
public class BlockInstance
{
    public BlockData blockData;
    public Quaternion rotation;
    public int rendererIndex;

    public BlockInstance(BlockData blockData, Quaternion rotation)
    {
        this.blockData = blockData;
        this.rotation = rotation;
        rendererIndex = -1;
    }

    public void SetRenderIndex(int i)
    {
        rendererIndex = i;
    }
}


