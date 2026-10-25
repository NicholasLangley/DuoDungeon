using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class planeLevelManager : MonoBehaviour
{
    public int level { get; set; }
    private Vector3 normal;

    [FormerlySerializedAs("yText")] [SerializeField]
    TextMeshProUGUI planeLevelText;
    [FormerlySerializedAs("blockPlacer")] [SerializeField]
    ObjectPlacer objectPlacer;

    // Start is called before the first frame update
    void OnEnable()
    {
        level = 0;
        normal = Vector3.up;
    }

    public void incrementLevel()
    {
        level++;
        updateIntersectionPlane();
    }

    public void decrementLevel()
    {
        level--;
        updateIntersectionPlane();
    }

    public void SetLevel(int y)
    {
        level = y;
        updateIntersectionPlane();
    }

    void SetNormal(Vector3 n)
    {
        normal = n;
        //todo maybe add some logic for determining what level to start
        level = 0;
        updateIntersectionPlane();
    }

    void updateIntersectionPlane()
    {
        planeLevelText.text = level.ToString();
        objectPlacer.SetLevelIntersectionPlane(normal, level);
    }
}
