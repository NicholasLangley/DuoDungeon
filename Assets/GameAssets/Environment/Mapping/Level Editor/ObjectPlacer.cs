using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPlacer : MonoBehaviour
{
    GameObject currentBlock;
    int currentBlockID;
    GameObject objectPlacementIndicator;
    //MeshRenderer blockPlacementMesh;
    Plane yLevelIntersectionPlane;
    public Map map;
    GameObject mapTransform;
    Vector3 intersectionPos;

    [SerializeField]
    GameObject redPlayerModelPrefab, bluePlayerModelPrefab;
    public GameObject redPlayerPlacementIndicator, bluePlayerPlacementIndicator;

    public enum objectType {none, block, redPlayer, bluePlayer, entity, environmentalEntity }
    objectType currentPlacementType;

    // Start is called before the first frame update
    void Start()
    {
        currentPlacementType = objectType.none;
        currentBlock = null;
        objectPlacementIndicator = null;
        yLevelIntersectionPlane = new Plane(); 
        SetYIntersectionPlane(0);
        mapTransform = new GameObject();
        redPlayerPlacementIndicator = Instantiate(redPlayerModelPrefab);
        bluePlayerPlacementIndicator = Instantiate(bluePlayerModelPrefab);
        intersectionPos = Vector3.negativeInfinity;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition));
        float distance;
        intersectionPos = Vector3.negativeInfinity;
        if (yLevelIntersectionPlane.Raycast(ray, out distance))
        {
            if (objectPlacementIndicator != null)
            {
                intersectionPos = ray.GetPoint(distance);
                objectPlacementIndicator.transform.position = Map.GetIntVector3(intersectionPos);
                unHideBlock();
            }
        }
        else { hideBlock(); }

    }

    public void SetBlock(GameObject block, int id)
    {
        currentBlock = block;
        currentBlockID = id;

        SetObjectIndicator(block);

        currentPlacementType = objectType.block;
    }

    public void SetPlayer(bool isRedPlayer)
    {
        if(isRedPlayer)
        {
            currentPlacementType = objectType.redPlayer;
            SetObjectIndicator(redPlayerModelPrefab);
        }
        else
        {
            currentPlacementType = objectType.bluePlayer;
            SetObjectIndicator(bluePlayerModelPrefab);
        }
    }

    void SetObjectIndicator(GameObject obj)
    {
        if (objectPlacementIndicator != null) { GameObject.Destroy(objectPlacementIndicator.gameObject); }
        objectPlacementIndicator = Instantiate(obj);
        objectPlacementIndicator.name = "Object Placement Indicator";
        objectPlacementIndicator.gameObject.AddComponent<GridLines>();
    }

    public void PlaceCurrentObject()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        
        if (currentPlacementType == objectType.block && currentBlock != null) { PlaceBlock(Map.GetIntVector3(intersectionPos)); }
        else if (currentPlacementType == objectType.redPlayer || currentPlacementType == objectType.bluePlayer) { PlacePlayer(Map.GetIntVector3(intersectionPos)); }
    }

    void PlaceBlock(Vector3Int position)
    {
        if (IsPositionOutOfBounds(position)) 
        {
            Debug.Log("block out of bounds");
            return;
        }

        GameObject newBlockObject = Instantiate(currentBlock, mapTransform.transform);
        Block newBlock = newBlockObject.GetComponent<Block>();
        newBlock.blockID = currentBlockID;
        newBlockObject.transform.position = position;
        newBlockObject.transform.rotation = objectPlacementIndicator.transform.rotation;
        map.AddBlock(position, newBlock);
    }

    void PlacePlayer(Vector3Int position)
    {
        if (IsPositionOutOfBounds(position))
        {
            Debug.Log("player out of bounds");
            return;
        }

        if (currentPlacementType == objectType.redPlayer)
        {
            map.redPlayerSpawn = position;
            redPlayerPlacementIndicator.transform.position = position;

            map.redPlayerSpawnRotation = objectPlacementIndicator.transform.rotation;
            redPlayerPlacementIndicator.transform.rotation = map.redPlayerSpawnRotation;
        }
        else
        {
            map.bluePlayerSpawn = position;
            bluePlayerPlacementIndicator.transform.position = position;

            map.bluePlayerSpawnRotation = objectPlacementIndicator.transform.rotation;
            bluePlayerPlacementIndicator.transform.rotation = map.bluePlayerSpawnRotation;
        }
    }

        void hideBlock()
    {
        if (objectPlacementIndicator != null) { objectPlacementIndicator.SetActive(false); }
    }

    void unHideBlock()
    {
        if (objectPlacementIndicator != null) { objectPlacementIndicator.SetActive(true); }
    }

    public void SetYIntersectionPlane(int yLevel)
    {
        yLevelIntersectionPlane.SetNormalAndPosition(Vector3.up, new Vector3(0, yLevel, 0));
    }

    public enum RotationDirection { RIGHT, LEFT, FORWARD, BACK,  RIGHTROLL, LEFTROLL}

    public void RotateObjectPlacement(RotationDirection dir)
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

        objectPlacementIndicator.transform.RotateAround(transform.position, rotationAxis, rotationAmount);
    }

    bool IsPositionOutOfBounds(Vector3 pos)
    {
        if (pos.x >= int.MaxValue || pos.x <= int.MinValue) { return true; }
        if (pos.y >= int.MaxValue || pos.y <= int.MinValue) { return true; }
        if (pos.z >= int.MaxValue || pos.z <= int.MinValue) { return true; }

        return false;
    }

}
