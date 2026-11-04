using UnityEngine;

public class Placeable : ScriptableObject
{
    [field: SerializeField] public string listID { get; set; }
    [field: SerializeField] public string baseID { get; set; }
    [field: SerializeField] public string varientID { get; set; }
}