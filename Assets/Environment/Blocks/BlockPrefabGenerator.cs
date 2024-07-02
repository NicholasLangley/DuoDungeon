using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//takes an ID and creates the corresponding prefab
[CreateAssetMenu(fileName = "BlockPrefabGeneratorScriptableObject", menuName = "ScriptableObjects/BlockPrefabGenerator")]
public class BlockPrefabGenerator : ScriptableObject
{
    [SerializeField]
    List<Block> blocks; //block0, block1;

    public BlockPrefabGenerator()
    {

    }

    public Block generatePrefab(int blockID)
    {
        return GameObject.Instantiate(blocks[blockID]);
        /*switch (blockID)
        {
            case 0:
                return GameObject.Instantiate(block0);

            case 1:
                return GameObject.Instantiate(block1);

            default:
                return null;
        }*/
    }
}
