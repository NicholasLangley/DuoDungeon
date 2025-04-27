using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentBlockDefinition
{
    [SerializeField]
    Vector3Int offsetFromCenter;
    [SerializeField]
    BlockSideDefinitions sides;
}

//Blocks that may move / occupy multiple grid spaces at once. Will calculate sub blocks that conform to the grid
public class ComplexBlock : MonoBehaviour, IPlaceable
{
    public string listID { get; set; }
    [field: SerializeField] public string baseID { get; set; }
    [field: SerializeField] public string varientID { get; set; }

    List<Block> gridBlocks;

    [SerializeField]
    List<ComponentBlockDefinition> componentBlocks;

    // Start is called before the first frame update
    void OnEnable()
    {
        gridBlocks = new List<Block>();
        CalculateGridBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateGridBlocks()
    {

    }

    public List<Block> GetGridBlocks()
    {
        return gridBlocks;
    }


}
