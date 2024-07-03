using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class yLevelManager : MonoBehaviour
{
    public int yLevel { get; set; }

    [SerializeField]
    TextMeshProUGUI yText;

    // Start is called before the first frame update
    void OnEnable()
    {
        yLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void incrementYLevel()
    {
        yLevel++;
        yText.text = yLevel.ToString();
    }

    public void decrementYLevel()
    {
        yLevel--;
        yText.text = yLevel.ToString();
    }

    public void setYLevel(int y)
    {
        yLevel = y;
        yText.text = yLevel.ToString();
    }
}
