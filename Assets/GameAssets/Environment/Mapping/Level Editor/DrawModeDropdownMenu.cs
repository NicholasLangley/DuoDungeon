using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawModeDropdownMenu : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] ObjectPlacer objectPlacer;
    // Start is called before the first frame update
    void Start()
    {
        PopulateDropdownMenu();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void PopulateDropdownMenu()
    {
        dropdown.options.Clear();
        
        string[] enumNames = Enum.GetNames(typeof(ObjectPlacer.DrawMode));
        List<string> dropdownOptions = new List<string>(enumNames);
        
        dropdown.AddOptions(dropdownOptions);
        dropdown.RefreshShownValue();
    }

    public void OnDropdownValueChanged(int index)
    {
        objectPlacer.setDrawMode((ObjectPlacer.DrawMode)index);
    }
}
