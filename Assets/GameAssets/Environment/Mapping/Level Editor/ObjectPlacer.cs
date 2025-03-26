using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour
{
    GameObject currentBlock;
    string currentBlockBaseID, currentBlockVarientID;

    GameObject objectPlacementIndicator;
    Quaternion startRotation, destRotation;
    float objectPlacementRotationTimer;
    [SerializeField]
    float objectPlacementRotationDuration = 0.5f;
    public bool isRotating;

    Plane yLevelIntersectionPlane;
    public Map map;
    GameObject mapTransform;
    Vector3 intersectionPos;

    [SerializeField]
    GameObject redPlayerModelPrefab, bluePlayerModelPrefab, placementArrow;
    public GameObject redPlayerPlacementIndicator, bluePlayerPlacementIndicator;

    public enum objectType {none, block, redPlayer, bluePlayer, entity, environmentalEntity }
    objectType currentPlacementType;

    BlockMasterList blockMasterList;

    GameObject placementIndicatorArrow;

    // Start is called before the first frame update
    void Start()
    {
        currentPlacementType = objectType.none;
        currentBlock = null;
        objectPlacementIndicator = null;
        objectPlacementRotationTimer = 0.0f;
        isRotating = false;
        yLevelIntersectionPlane = new Plane(); 
        SetYIntersectionPlane(0);
        mapTransform = new GameObject();
        redPlayerPlacementIndicator = Instantiate(redPlayerModelPrefab);
        bluePlayerPlacementIndicator = Instantiate(bluePlayerModelPrefab);
        intersectionPos = Vector3.negativeInfinity;
        placementIndicatorArrow = Instantiate(placementArrow);
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
        if(isRotating)
        {
            objectPlacementRotationTimer += Time.deltaTime;
            objectPlacementIndicator.transform.rotation = Quaternion.Lerp(startRotation, destRotation, objectPlacementRotationTimer / objectPlacementRotationDuration);
            if (objectPlacementRotationTimer > objectPlacementRotationDuration)
            {
                isRotating = false;
            }
        }
    }

    public void SetBlockList(BlockMasterList list)
    {
        blockMasterList = list;
    }

    public void SetBlock(GameObject block, string baseID, string varientID)
    {
        currentBlock = block;
        currentBlockBaseID = baseID;
        currentBlockVarientID = varientID;

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

        placementIndicatorArrow.transform.SetParent(objectPlacementIndicator.transform, false);
    }

    public Command PlaceCurrentObject()
    {
        if (EventSystem.current.IsPointerOverGameObject() || isRotating) { return null; }

        Command cmd = null;

        if (currentPlacementType == objectType.block && currentBlock != null) { cmd = GetPlaceBlockCommand(Map.GetIntVector3(intersectionPos), objectPlacementIndicator.transform.rotation, currentBlockBaseID, currentBlockVarientID); }
        else if (currentPlacementType == objectType.redPlayer || currentPlacementType == objectType.bluePlayer) { cmd = GetPlacePlayerCommand(Map.GetIntVector3(intersectionPos), objectPlacementIndicator.transform.rotation, (currentPlacementType == objectType.redPlayer)); }

        if (cmd != null) { cmd.Execute(); }
        return cmd;
    }

    Command GetPlaceBlockCommand(Vector3Int position, Quaternion rotation, string baseID, string varientID)
    {
        if (IsPositionOutOfBounds(position)) 
        {
            Debug.Log("block out of bounds");
            return null;
        }

        Block preExistingBlock = map.GetBlock(position);
        PlaceBlockCommand cmd;
        if(preExistingBlock != null)
        {
            cmd = new PlaceBlockCommand(this, position, baseID, varientID, rotation, preExistingBlock.baseID, preExistingBlock.varientID, preExistingBlock.gameObject.transform.rotation);
        }
        else
        {
            cmd = new PlaceBlockCommand(this, position, baseID, varientID, rotation);
        }
        return cmd;
    }

    public void PlaceBlock(Vector3Int position, Quaternion rotation, string baseID, string varientID)
    {
        if(string.Compare(baseID, "DELETE") == 0)
        {
            RemoveBlock(position);
            return;
        }

        GameObject newBlockObject = Instantiate(blockMasterList.GetBlock(baseID, varientID), mapTransform.transform);
        Block newBlock = newBlockObject.GetComponent<Block>();
        newBlockObject.transform.position = position;
        newBlockObject.transform.rotation = rotation;
        map.AddBlock(position, newBlock);
    }

    public void RemoveBlock(Vector3Int position)
    {
        map.RemoveBlock(position);
        return;
    }

    Command GetPlacePlayerCommand(Vector3Int position, Quaternion rotation, bool isRedPlayer)
    {
        if (IsPositionOutOfBounds(position))
        {
            Debug.Log("player out of bounds");
            return null;
        }

        Block preExistingBlock = map.GetBlock(position);
        Vector3Int oldPlayerPos = isRedPlayer ? Map.GetIntVector3(redPlayerPlacementIndicator.transform.position) : Map.GetIntVector3(bluePlayerPlacementIndicator.transform.position);
        Quaternion oldPlayerRot = isRedPlayer ? redPlayerPlacementIndicator.transform.rotation : bluePlayerPlacementIndicator.transform.rotation;
        PlacePlayerCommand cmd;
        if (preExistingBlock != null)
        {
            cmd = new PlacePlayerCommand(this, position, rotation, isRedPlayer, oldPlayerPos, oldPlayerRot, preExistingBlock.baseID, preExistingBlock.varientID, preExistingBlock.gameObject.transform.rotation);
        }
        else
        {
            cmd = new PlacePlayerCommand(this, position, rotation, isRedPlayer, oldPlayerPos, oldPlayerRot);
        }
        return cmd;
    }

    public void PlacePlayer(Vector3Int position, Quaternion rotation, bool isRedPlayer)
    {
        map.RemoveBlock(position);
        if (isRedPlayer)
        {
            map.redPlayerSpawn = position;
            redPlayerPlacementIndicator.transform.position = position;

            map.redPlayerSpawnRotation = rotation;
            redPlayerPlacementIndicator.transform.rotation = rotation;
        }
        else
        {
            map.bluePlayerSpawn = position;
            bluePlayerPlacementIndicator.transform.position = position;

            map.bluePlayerSpawnRotation = rotation;
            bluePlayerPlacementIndicator.transform.rotation = rotation;
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
        if (isRotating) { return; }
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

            case RotationDirection.LEFTROLL:
                rotationAxis = Vector3.forward;
                break;

            default: //RIGHTROLL
                rotationAxis = Vector3.forward;
                rotationAmount = -90f;
                break;
        }

       
        startRotation = objectPlacementIndicator.transform.rotation;
        objectPlacementIndicator.transform.RotateAround(transform.position, rotationAxis, rotationAmount);
        destRotation = objectPlacementIndicator.transform.rotation;
        objectPlacementIndicator.transform.rotation = startRotation;
        objectPlacementRotationTimer = 0.0f;
        isRotating = true;
    }

    bool IsPositionOutOfBounds(Vector3 pos)
    {
        if (pos.x >= int.MaxValue || pos.x <= int.MinValue) { return true; }
        if (pos.y >= int.MaxValue || pos.y <= int.MinValue) { return true; }
        if (pos.z >= int.MaxValue || pos.z <= int.MinValue) { return true; }

        return false;
    }

}
