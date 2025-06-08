using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable: MonoBehaviour
{
    public string listID { get; set; }
    [field: SerializeField] public string baseID { get; set; }
    [field: SerializeField] public string varientID { get; set; }
}
