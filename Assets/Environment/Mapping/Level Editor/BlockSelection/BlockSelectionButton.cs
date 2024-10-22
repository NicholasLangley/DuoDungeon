using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSelectionButton : MonoBehaviour
{
    public string blockName { get; set; }
    public int blockId { get; set; }

    [SerializeField]
    TextMeshProUGUI text;

    public GameObject block;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlock(int id, GameObject B)
    {
        blockId = id;
        block = B;
        text.text = block.name;
    }
    
}
