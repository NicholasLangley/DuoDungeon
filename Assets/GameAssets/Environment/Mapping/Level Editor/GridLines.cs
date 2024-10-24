using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLines : MonoBehaviour
{
    int gridSize = 2;
    float lineWidth = 0.01f;

    List<LineRenderer> lines;

    // Start is called before the first frame update
    void Start()
    {
        lines = new List<LineRenderer>();
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {

                #region x axis lines
                //x lines
                //+y
                //+z
                GameObject newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                LineRenderer newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(gridSize, j + 0.5f, i + 0.5f));
                newLine.SetPosition(1, new Vector3(-gridSize, j+ 0.5f, i + 0.5f));
                lines.Add(newLine);

                //+y
                //-z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(gridSize, j + 0.5f, -0.5f - i));
                newLine.SetPosition(1, new Vector3(-gridSize, j + 0.5f, -0.5f - i));
                lines.Add(newLine);

                //-y
                //+z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(gridSize, -0.5f - j, i + 0.5f));
                newLine.SetPosition(1, new Vector3(-gridSize, -0.5f - j, i + 0.5f));
                lines.Add(newLine);

                //-y
                //-z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(gridSize, -0.5f - j, -0.5f - i));
                newLine.SetPosition(1, new Vector3(-gridSize, -0.5f - j, -0.5f - i));
                lines.Add(newLine);
                #endregion

                #region y axis lines

                //y lines
                //+x
                //+z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(j + 0.5f, gridSize, i + 0.5f));
                newLine.SetPosition(1, new Vector3(j + 0.5f, -gridSize, i + 0.5f));
                lines.Add(newLine);

                //-x
                //+z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(-0.5f - j, gridSize, i + 0.5f));
                newLine.SetPosition(1, new Vector3(-0.5f - j, -gridSize, i + 0.5f));
                lines.Add(newLine);

                //+x
                //-z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(j + 0.5f, gridSize, -0.5f - i));
                newLine.SetPosition(1, new Vector3(j + 0.5f, -gridSize, -0.5f - i));
                lines.Add(newLine);

                //-x
                //-z
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(-0.5f - j, gridSize, -0.5f - i));
                newLine.SetPosition(1, new Vector3(-0.5f - j, -gridSize, -0.5f - i));
                lines.Add(newLine);

                #endregion

                #region z axis lines

                //z lines
                //+x
                //+y
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(j + 0.5f, i + 0.5f, gridSize));
                newLine.SetPosition(1, new Vector3(j + 0.5f, i + 0.5f, -gridSize));
                lines.Add(newLine);

                //-x
                //+y
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(-0.5f - j, i + 0.5f, gridSize));
                newLine.SetPosition(1, new Vector3(-0.5f - j, i + 0.5f, -gridSize));
                lines.Add(newLine);

                //+x
                //-y
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(j + 0.5f, -0.5f - i, gridSize));
                newLine.SetPosition(1, new Vector3(j + 0.5f, -0.5f - i, -gridSize));
                lines.Add(newLine);

                //-x
                //-y
                newLineObject = new GameObject();
                newLineObject.transform.parent = transform;
                newLineObject.transform.position = transform.position;
                newLine = newLineObject.AddComponent<LineRenderer>();
                newLine.startWidth = lineWidth;
                newLine.endWidth = lineWidth;
                newLine.useWorldSpace = false;
                newLine.SetPosition(0, new Vector3(-0.5f - j, -0.5f - i, gridSize));
                newLine.SetPosition(1, new Vector3(-0.5f - j, -0.5f - i, -gridSize));
                lines.Add(newLine);

                #endregion
            }
        }


    }
}
