using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour
{
    //Map building
    [SerializeField]
    Map map;
    MapBuilder mapBuilder;
    [SerializeField]
    BlockList blockList;
    BlockBuilder blockBuilder;

    // Start is called before the first frame update
    void Start()
    {
        blockBuilder = new BlockBuilder(blockList);
        mapBuilder = new MapBuilder(blockBuilder, map);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
