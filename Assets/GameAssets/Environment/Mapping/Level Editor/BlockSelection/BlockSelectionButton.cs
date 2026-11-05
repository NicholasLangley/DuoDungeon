using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSelectionButton : MonoBehaviour
{
    public string blockName { get; set; }

    [SerializeField]
    TextMeshProUGUI text;

    public Placeable placeable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlock(Placeable placeable, string name)
    {
        this.placeable = placeable;
        text.text = name;
    }
    
}
