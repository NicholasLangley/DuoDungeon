using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    Block currentBlock;
    int currentBlockID;
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && currentBlock != null)
        {
            PlaceBlock(Map.GetIntVector3(intersectionPos));
        }


        //BLOCK ROTATION
        //probably rewrite this later
        if(Input.GetKeyDown(KeyCode.I))
        {
            RotateBlockPlacement(RotationDirection.FORWARD);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            RotateBlockPlacement(RotationDirection.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            RotateBlockPlacement(RotationDirection.BACK);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            RotateBlockPlacement(RotationDirection.RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            RotateBlockPlacement(RotationDirection.LEFTROLL);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            RotateBlockPlacement(RotationDirection.RIGHTROLL);
        }

    }

    public void setBlock(Block block, int id)
    {
        currentBlock = block;
        currentBlockID = id;
        if (blockPlacementIndicator != null) { GameObject.Destroy(blockPlacementIndicator.gameObject); }
        blockPlacementIndicator = GameObject.Instantiate(block);
        blockPlacementIndicator.name = "blockPlacementIndicator";
        blockPlacementMesh = blockPlacementIndicator.GetComponent<MeshRenderer>();
        blockPlacementIndicator.gameObject.AddComponent<GridLines>();
    }

    public void SetYIntersectionPlane(int yLevel)
    {
        yLevelIntersectionPlane.SetNormalAndPosition(Vector3.up, new Vector3(0, yLevel, 0));
    }

    void PlaceBlock(Vector3Int position)
    {
        if (IsPositionOutOfBounds(position)) 
        {
            Debug.Log("block out of bounds");
            return;
        }

        Block newBlock = Instantiate(currentBlock, map.transform);
        newBlock.blockID = currentBlockID;
        newBlock.transform.position = position;
        newBlock.transform.rotation = blockPlacementIndicator.transform.rotation;
        map.AddBlock(position, newBlock);
    }

    void hideBlock()
    {
        blockPlacementMesh.enabled = false;
    }

    void unHideBlock()
    {
        blockPlacementMesh.enabled = true;
    }

    enum RotationDirection { RIGHT, LEFT, FORWARD, BACK,  RIGHTROLL, LEFTROLL}

    void RotateBlockPlacement(RotationDirection dir)
    {
        Vector3 rotationAxis;
        float rotationAmount = 90;

        switch (dir)
        {
            case RotationDirection.RIGHT:
                rotationAxis = Vector3.up;
                break;

            case RotationDirection.LEFT:
                rotationAxis = Vector3.up;
                rotationAmount = -90f;
                break;

            case RotationDirection.FORWARD:
                rotationAxis = Vector3.right;
                break;

            case RotationDirection.BACK:
                rotationAxis = Vector3.right;
                rotationAmount = -90f;
                break;

            case RotationDirection.RIGHTROLL:
                rotationAxis = Vector3.forward;
                break;

            default: //LEFTROLL
                rotationAxis = Vector3.forward;
                rotationAmount = -90f;
                break;
        }

        blockPlacementIndicator.transform.RotateAround(transform.position, rotationAxis, rotationAmount);
    }

    bool IsPositionOutOfBounds(Vector3 pos)
    {
        if (pos.x >= int.MaxValue || pos.x <= int.MinValue) { return true; }
        if (pos.y >= int.MaxValue || pos.y <= int.MinValue) { return true; }
        if (pos.z >= int.MaxValue || pos.z <= int.MinValue) { return true; }

        return false;
    }

}
