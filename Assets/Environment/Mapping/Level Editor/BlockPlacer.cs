using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    Block currentBlock;
    Block blockPlacementIndicator;
    MeshRenderer blockPlacementMesh;
    Plane yLevelIntersectionPlane;
    [SerializeField]
    Map map;

    // Start is called before the first frame update
    void Start()
    {
        currentBlock = null;
        blockPlacementIndicator = null;
        yLevelIntersectionPlane = new Plane();
        SetYIntersectionPlane(0);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition));
        float distance;
        Vector3 intersectionPos = Vector3.negativeInfinity;
        if (yLevelIntersectionPlane.Raycast(ray, out distance))
        {
            if (blockPlacementIndicator != null)
            {
                intersectionPos = ray.GetPoint(distance);
                blockPlacementIndicator.transform.position = Map.GetIntVector3(intersectionPos);
                unHideBlock();
            }
        }
        else if (blockPlacementMesh != null) { hideBlock(); }

        if (Input.GetKeyDown(KeyCode.Mouse0) && currentBlock != null && intersectionPos != Vector3.negativeInfinity)
        {
            PlaceBlock(Map.GetIntVector3(intersectionPos));
        }

    }

    public void setBlock(Block block)
    {
        currentBlock = block;
        if (blockPlacementIndicator != null) { GameObject.Destroy(blockPlacementIndicator.gameObject); }
        blockPlacementIndicator = GameObject.Instantiate(block, transform);
        blockPlacementMesh = blockPlacementIndicator.GetComponent<MeshRenderer>();
    }

    public void SetYIntersectionPlane(int yLevel)
    {
        yLevelIntersectionPlane.SetNormalAndPosition(Vector3.up, new Vector3(0, yLevel, 0));
    }

    void PlaceBlock(Vector3Int position)
    {
        Block newBlock = Instantiate(currentBlock, map.transform);
        newBlock.transform.position = position;
        map.AddBlock(position, currentBlock);
    }

    void hideBlock()
    {
        blockPlacementMesh.enabled = false;
    }

    void unHideBlock()
    {
        blockPlacementMesh.enabled = true;
    }

}
