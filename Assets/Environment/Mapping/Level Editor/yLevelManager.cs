using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class yLevelManager : MonoBehaviour
{
    public int yLevel { get; set; }

    [SerializeField]
    TextMeshProUGUI yText;
    [SerializeField]
    BlockPlacer blockPlacer;

    // Start is called before the first frame update
    void OnEnable()
    {
        yLevel = 0;
    }

    public void incrementYLevel()
    {
        yLevel++;
        updateYLevel();
    }

    public void decrementYLevel()
    {
        yLevel--;
        updateYLevel();
    }

    public void setYLevel(int y)
    {
        yLevel = y;
        updateYLevel();
    }

    void updateYLevel()
    {
        yText.text = yLevel.ToString();
        blockPlacer.SetYIntersectionPlane(yLevel);
    }
}
