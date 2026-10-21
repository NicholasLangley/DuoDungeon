using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour
{
    GameObject currentBlock;
    string currentListID, currentBlockBaseID, currentBlockVarientID;

    GameObject objectPlacementIndicator;
    Quaternion startRotation, destRotation;
    float objectPlacementRotationTimer;
    [SerializeField] float objectPlacementRotationDuration = 0.5f;
    public bool isRotating;

    Plane yLevelIntersectionPlane;
    public Map map;
    GameObject mapTransform;
    Vector3 intersectionPos;

    [SerializeField] GameObject redPlayerModelPrefab, bluePlayerModelPrefab, placementArrow;
    public GameObject redPlayerPlacementIndicator, bluePlayerPlacementIndicator;

    public enum ObjectType
    {
        none,
        block,
        wall,
        redPlayer,
        bluePlayer,
        entity,
        environmentalEntity
    }

    ObjectType _currentPlacementType;

    public enum DrawMode
    {
        point,
        pencil,
        line,
        box,
        boxfill,
        circle,
        circlefill
    }

    DrawMode _currentDrawMode;
    private bool _currentlyDrawing;
    private bool pointDrawModeLimited;

    UltimateList ultimateList;
    public List<string> wallListIDs;

    GameObject placementIndicatorArrow;

    // Start is called before the first frame update
    void Start()
    {
        _currentPlacementType = ObjectType.none;
        currentBlock = null;
        objectPlacementIndicator = null;
        objectPlacementRotationTimer = 0.0f;
        isRotating = false;
        yLevelIntersectionPlane = new Plane();
        SetYIntersectionPlane(Vector3.up, 0);
        mapTransform = new GameObject();
        redPlayerPlacementIndicator = Instantiate(redPlayerModelPrefab);
        bluePlayerPlacementIndicator = Instantiate(bluePlayerModelPrefab);
        intersectionPos = Vector3.negativeInfinity;
        placementIndicatorArrow = Instantiate(placementArrow);
        _currentDrawMode = DrawMode.pencil;
        _currentlyDrawing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            HandleRotation();
        }
        //CalculateProjectedGhostPlacements();
        PlacePlacementIndicatorGhosts();
    }

public void SetBlockList(UltimateList list)
    {
        ultimateList = list;
    }

    public void SetBlock(GameObject block, string listID, string baseID, string varientID)
    {
        currentBlock = block;
        currentListID = listID;
        currentBlockBaseID = baseID;
        currentBlockVarientID = varientID;

        if (wallListIDs.Contains(listID))
        {
            _currentPlacementType = ObjectType.wall;
            SetWallPlacerMode(true);
            SetObjectIndicator(block);
        }
        else
        {
            SetObjectIndicator(block);
            _currentPlacementType = ObjectType.block;
            SetWallPlacerMode(false);
            
            //for now limit complex blocks to point drawmode,
            //since they can occupy multiple blocks and I don't want to deal with draw collisions
            // could maybe add a variable to the blocks on if point mode is enabled or not?
            ComplexBlock complexBlockBeingPlaced = objectPlacementIndicator.GetComponent<ComplexBlock>();
            if (complexBlockBeingPlaced != null)
            {
                pointDrawModeLimited = true;
                _currentDrawMode = DrawMode.point;
            }
            else
            {
                pointDrawModeLimited = false;
                //TODO maybe save previous draw mode from before enforcing point mode?
            }
        }
    }

    public void SetPlayer(bool isRedPlayer)
    {
        if(isRedPlayer)
        {
            _currentPlacementType = ObjectType.redPlayer;
            SetObjectIndicator(redPlayerModelPrefab);
        }
        else
        {
            _currentPlacementType = ObjectType.bluePlayer;
            SetObjectIndicator(bluePlayerModelPrefab);
        }
        SetWallPlacerMode(false);
        pointDrawModeLimited = true;
        _currentDrawMode = DrawMode.point;
    }

    void SetObjectIndicator(GameObject obj)
    {
        if (objectPlacementIndicator != null) { GameObject.Destroy(objectPlacementIndicator.gameObject); }
        objectPlacementIndicator = Instantiate(obj);
        objectPlacementIndicator.name = "Object Placement Indicator";
        objectPlacementIndicator.gameObject.AddComponent<GridLines>();

        placementIndicatorArrow.transform.SetParent(objectPlacementIndicator.transform, false);
    }

    void SetWallPlacerMode(bool isWallPlacerMode)
    {
        if (isWallPlacerMode)
        {
            Debug.Log("TODO enable wallPlacerMode");
        }
        else
        {
            Debug.Log("TODO disable wallPlacerMode");
        }
    }

    #region DrawingCalculations

    //possible performance improvement: calculate every x frames
    void CalculateProjectedGhostPlacements()
    {
        //single point where intersecting plane
        if (!_currentlyDrawing || _currentDrawMode == DrawMode.point)
        {
            CalculatePointModeGhost();
        }
        else if (_currentDrawMode == DrawMode.pencil)
        {
            //no ghosts, since we're placing as we go
            hideBlock();
        }
        else if (_currentDrawMode == DrawMode.line)
        {
            //start vs end point. linear interpolation?
        }
        else if (_currentDrawMode == DrawMode.box)
        {
            //start vs end point. on current reference plane
        }
        else if (_currentDrawMode == DrawMode.boxfill)
        {
            //start vs end point. on current reference plane
        }
        else if (_currentDrawMode == DrawMode.circle)
        {
            //start vs end point. radius or diameter? circle algorithm? ovals?
        }
        else if (_currentDrawMode == DrawMode.circlefill)
        {
            //start vs end point. radius or diameter? circle algorithm? ovals?
        }
    }

    void CalculatePointModeGhost()
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
        else
        {
            hideBlock();
        }
    }
    
    #endregion

    //possible performance improvement: render every x frames
    void PlacePlacementIndicatorGhosts()
    {
        if (!_currentlyDrawing)
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
            else
            {
                hideBlock();
            }
        }
    }
    void HandleRotation() 
    {
        //todo rotate each ghost individually

        objectPlacementRotationTimer += Time.deltaTime;
        objectPlacementIndicator.transform.rotation = Quaternion.Lerp(startRotation, destRotation,
            objectPlacementRotationTimer / objectPlacementRotationDuration);
        if (objectPlacementRotationTimer > objectPlacementRotationDuration)
        {
            isRotating = false;
        }
    }

    public void BeginDrawing()
    {
        _currentlyDrawing = true;
    }

    public void FinishDrawing()
    {

        PlaceProjectedBlocks();
        _currentlyDrawing = false;
    }

    void PlaceProjectedBlocks()
    {
        //place blocks as it goes, TODO figure out how to make these a commandGroup for simple undo
        if (_currentDrawMode == DrawMode.pencil) { return; }
        //depending on draw mode (though i guess all can be grouped, even if its just one block)
        //create command group
        //for each projected block
        //  Create command
        //  add to commandgroup
        //  execute command group (or maybe return it to the level editor controller, need to remember how the command process fully works)
    }
    
    //todo swap to Place ProjectedBlocks
    public List<Command> PlaceCurrentObject()
    {
        if (EventSystem.current.IsPointerOverGameObject() || isRotating) { return null; }

        List<Command> commands = new List<Command>();

        if (_currentPlacementType == ObjectType.block && currentBlock != null) { commands = GetPlaceBlockCommands(Map.GetIntVector3(intersectionPos), objectPlacementIndicator.transform.rotation, currentListID, currentBlockBaseID, currentBlockVarientID); }
        else if (_currentPlacementType == ObjectType.wall && currentBlock != null) { commands = GetPlaceWallCommands(intersectionPos, objectPlacementIndicator.transform.rotation, currentListID, currentBlockBaseID, currentBlockVarientID); }
        else if (_currentPlacementType == ObjectType.redPlayer || _currentPlacementType == ObjectType.bluePlayer) { commands = GetPlacePlayerCommand(Map.GetIntVector3(intersectionPos), objectPlacementIndicator.transform.rotation, (_currentPlacementType == ObjectType.redPlayer)); }

        if (commands.Count > 0) 
        {
            foreach (Command cmd in commands) { cmd.Execute(); }
        }
        return commands;
    }

    List<Command> GetPlaceBlockCommands(Vector3Int position, Quaternion rotation, string listID, string baseID, string varientID)
    {
        if (IsPositionOutOfBounds(position))
        {
            Debug.Log("block out of bounds");
            return new List<Command>();
        }
        List<Command> commands = new List<Command>();

        List<Vector3Int> positionsToCheck;
        ComplexBlock complexBlockBeingPlaced = objectPlacementIndicator.GetComponent<ComplexBlock>();
        if (complexBlockBeingPlaced != null)
        {
            positionsToCheck = complexBlockBeingPlaced.getSubBlockPositions(position);
        }
        else
        {
            positionsToCheck = new List<Vector3Int>();
            positionsToCheck.Add(position);
        }

        List<ComplexBlock> alreadyCollidedComplexBlocks = new List<ComplexBlock>();
        bool canPlaceBlock = true;
        foreach (Vector3Int positionCheck in positionsToCheck)
        {
            //don't place block if colliding with player?
            //todo maybe allow it and delete the player, but then don't allow this kind of map to be playable?
            if (positionCheck == Map.GetIntVector3(redPlayerPlacementIndicator.transform.position) || positionCheck == Map.GetIntVector3(bluePlayerPlacementIndicator.transform.position))
            {
                commands.Clear();
                canPlaceBlock = false;
                break;
            }

            Block preExistingBlock = map.GetBlockAtGridPosition(positionCheck, null, Vector3.down);
            if (preExistingBlock != null)
            {
                string preExistingListID;
                string preExistingBaseID;
                string preExistingVarID;

                Vector3Int rootPosition = positionCheck;
                ComplexBlock preExistingComplexBlock = preExistingBlock.GetComponentInParent<ComplexBlock>();
                if (preExistingComplexBlock != null)
                {
                    //skip rest of loop iteration if already collided with block
                    if (alreadyCollidedComplexBlocks.Contains(preExistingComplexBlock)) { continue; }
                    alreadyCollidedComplexBlocks.Add(preExistingComplexBlock);

                    rootPosition = Map.GetIntVector3(preExistingComplexBlock.transform.position);
                    preExistingListID = preExistingComplexBlock.listID;
                    preExistingBaseID = preExistingComplexBlock.baseID;
                    preExistingVarID = preExistingComplexBlock.varientID;
                }
                else
                {
                    preExistingListID = preExistingBlock.listID;
                    preExistingBaseID = preExistingBlock.baseID;
                    preExistingVarID = preExistingBlock.varientID;
                }
                RemoveBlockCommand removeCmd = new RemoveBlockCommand(this, rootPosition, preExistingListID, preExistingBaseID, preExistingVarID, preExistingBlock.gameObject.transform.rotation);
                commands.Add(removeCmd);
            }
        }

        if (canPlaceBlock)
        {
            PlaceBlockCommand cmd = new PlaceBlockCommand(this, position, listID, baseID, varientID, rotation);
            commands.Add(cmd);
        }

        return commands;
    }

    public void PlaceBlock(Vector3Int position, Quaternion rotation, string listID, string baseID, string varientID)
    {
        GameObject newBlockObject = Instantiate(ultimateList.GetMasterList(listID).GetBlock(baseID, varientID), mapTransform.transform);
        Block newBlock = newBlockObject.GetComponent<Block>();
        if (newBlock != null)
        {
            newBlock.listID = listID;
            map.AddStaticBlock(position, newBlock);
        }
        else
        {
            ComplexBlock complexBlock = newBlockObject.GetComponent <ComplexBlock>();
            complexBlock.listID = listID;
            map.AddComplexBlock(complexBlock);
        }
        newBlockObject.transform.position = position;
        newBlockObject.transform.rotation = rotation;
    }

    public void RemoveBlock(Vector3Int position)
    {
        map.RemoveBlockAtLocation(position);
        //TODO figure out why blocks are going to 0,0,0 or elsewhere?
    }

    List<Command> GetPlaceWallCommands(Vector3 position, Quaternion rotation, string listID, string baseID, string varientID)
    {
        if (IsPositionOutOfBounds(position))
        {
            Debug.Log("block out of bounds");
            return new List<Command>();
        }
        List<Command> commands = new List<Command>();
        
        //TODO
        Debug.Log("get place wall command");
        return commands;
    }

    public void PlaceWall(Vector3Int position, Quaternion rotation, string listID, string baseID, string varientID)
    {
        Debug.Log("Placing wall at " + position);
        //TODO
    }
    
    public void RemoveWall(Vector3Int position)
    {
        Debug.Log("Removing wall at " + position);
        //TODO
    }

    List<Command> GetPlacePlayerCommand(Vector3Int position, Quaternion rotation, bool isRedPlayer)
    {
        if (IsPositionOutOfBounds(position))
        {
            Debug.Log("player out of bounds");
            return null;
        }
        List<Command> commands = new List<Command>();

        Block preExistingBlock = map.GetBlockAtGridPosition(position, null, Vector3.down);

        Vector3Int oldPlayerPos = isRedPlayer ? Map.GetIntVector3(redPlayerPlacementIndicator.transform.position) : Map.GetIntVector3(bluePlayerPlacementIndicator.transform.position);
        Quaternion oldPlayerRot = isRedPlayer ? redPlayerPlacementIndicator.transform.rotation : bluePlayerPlacementIndicator.transform.rotation;

        if (preExistingBlock != null)
        {
            Vector3Int rootPosition = position;

            string preExistingListID;
            string preExistingBaseID;
            string preExistingVarID;

            ComplexBlock preExistingComplexBlock = preExistingBlock.GetComponentInParent<ComplexBlock>();
            if (preExistingComplexBlock != null)
            {
                rootPosition = Map.GetIntVector3(preExistingComplexBlock.transform.position);
                preExistingListID = preExistingComplexBlock.listID;
                preExistingBaseID = preExistingComplexBlock.baseID;
                preExistingVarID = preExistingComplexBlock.varientID;
            }
            else
            {
                preExistingListID = preExistingBlock.listID;
                preExistingBaseID = preExistingBlock.baseID;
                preExistingVarID = preExistingBlock.varientID;
            }

            RemoveBlockCommand removeCmd = new RemoveBlockCommand(this, rootPosition, preExistingListID, preExistingBaseID, preExistingVarID, preExistingBlock.gameObject.transform.rotation);
            commands.Add(removeCmd);
        }
        //checked for player collision
        if(isRedPlayer && position == Map.GetIntVector3(bluePlayerPlacementIndicator.transform.position))
        {
            PlacePlayerCommand moveOtherPlayerCmd = new PlacePlayerCommand(this, oldPlayerPos, oldPlayerRot, !isRedPlayer, Map.GetIntVector3(bluePlayerPlacementIndicator.transform.position), bluePlayerPlacementIndicator.transform.rotation);
            commands.Add(moveOtherPlayerCmd);
        }
        else if (!isRedPlayer && position == Map.GetIntVector3(redPlayerPlacementIndicator.transform.position))
        {
            PlacePlayerCommand moveOtherPlayerCmd = new PlacePlayerCommand(this, oldPlayerPos, oldPlayerRot, isRedPlayer, Map.GetIntVector3(redPlayerPlacementIndicator.transform.position), redPlayerPlacementIndicator.transform.rotation);
            commands.Add(moveOtherPlayerCmd);
        }

        PlacePlayerCommand cmd = new PlacePlayerCommand(this, position, rotation, isRedPlayer, oldPlayerPos, oldPlayerRot);
        commands.Add(cmd);

        return commands;
    }

    public void PlacePlayer(Vector3Int position, Quaternion rotation, bool isRedPlayer)
    {
        map.RemoveStaticBlock(position);
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

    public void SetYIntersectionPlane(Vector3 normal, int yLevel)
    {
        yLevelIntersectionPlane.SetNormalAndPosition(normal, new Vector3(0, yLevel, 0));
    }

    public enum RotationDirection { RIGHT, LEFT, FORWARD, BACK,  RIGHTROLL, LEFTROLL}

    public void RotateObjectPlacement(RotationDirection dir)
    {
        if (isRotating || _currentPlacementType == ObjectType.wall) { return; }
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

    public void setDrawMode(DrawMode drawMode)
    {
        _currentDrawMode = drawMode;
    }
    
}
