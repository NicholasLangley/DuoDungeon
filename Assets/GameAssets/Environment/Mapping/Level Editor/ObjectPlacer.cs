using System;
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

    Plane levelIntersectionPlane;
    public Map map;
    GameObject mapTransform;

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
    
    Vector3Int _drawStartPosition;
    List<Vector3Int> _ghostDrawPositionList;
    private GameObject _ghostParent;
    private GameObject _ghostSelectedBlock;
    List<Transform> _ghostTransformList;

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
        levelIntersectionPlane = new Plane();
        SetLevelIntersectionPlane(Vector3.up, 0);
        mapTransform = new GameObject();
        redPlayerPlacementIndicator = Instantiate(redPlayerModelPrefab);
        bluePlayerPlacementIndicator = Instantiate(bluePlayerModelPrefab);
        placementIndicatorArrow = Instantiate(placementArrow);
        _currentDrawMode = DrawMode.point;
        _currentlyDrawing = false;
        _ghostDrawPositionList = new List<Vector3Int>();
        _ghostTransformList = new List<Transform>();
        _ghostParent = new GameObject(name: "ghost parent");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            HandleRotation();
        }
        CalculateProjectedGhostPlacements();
        PlacePlacementIndicatorGhosts();
    }
    
    #region Set Objects
    
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
            SetProjectedGhostObject(block);
        }
        else
        {
            SetProjectedGhostObject(block);
            _currentPlacementType = ObjectType.block;
            SetWallPlacerMode(false);
            
            //for now limit complex blocks to point drawmode,
            //since they can occupy multiple blocks and I don't want to deal with draw collisions
            // could maybe add a variable to the blocks on if point mode is enabled or not?
            ComplexBlock complexBlockBeingPlaced = _ghostSelectedBlock.GetComponent<ComplexBlock>();
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
            SetProjectedGhostObject(redPlayerModelPrefab);
        }
        else
        {
            _currentPlacementType = ObjectType.bluePlayer;
            SetProjectedGhostObject(bluePlayerModelPrefab);
        }
        SetWallPlacerMode(false);
        pointDrawModeLimited = true;
        _currentDrawMode = DrawMode.point;
    }

    void SetProjectedGhostObject(GameObject obj)
    {
        if (_ghostSelectedBlock != null) { GameObject.Destroy(_ghostSelectedBlock.gameObject); }
        _ghostSelectedBlock = Instantiate(obj);
        _ghostSelectedBlock.name = "Ghost Block Blueprint";
        _ghostSelectedBlock.transform.parent = _ghostParent.transform;
        //clear ghost list so it can be recreated with the new block type
        while (_ghostTransformList.Count > 0)
        {
            Transform ghostToExorcise = _ghostTransformList[0];
            _ghostTransformList.RemoveAt(0);
            GameObject.Destroy(ghostToExorcise.gameObject);
        }
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
    
    public void setDrawMode(DrawMode drawMode)
    {
        _currentDrawMode = drawMode;
    }
    
    #endregion

    #region Drawing
    public List<Command> BeginDrawing()
    {
        //point mode will place immediately
        if (_currentDrawMode == DrawMode.point)
        {
            return PlaceProjectedBlocks();
        }
        _currentlyDrawing = true;
        _drawStartPosition = Map.GetIntVector3(GetCurrentIntersectPoint());
        return null;
    }

    public List<Command> FinishDrawing()
    {
        //point mode already placed on press
        if (_currentDrawMode == DrawMode.point)
        {
            return null;
        }
        _currentlyDrawing = false;
        return PlaceProjectedBlocks();
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
            hideGhosts();
        }
        else if (_currentDrawMode == DrawMode.line)
        {
            //start vs end point. linear interpolation?
            CalculateLineModeGhosts();
        }
        else if (_currentDrawMode == DrawMode.box)
        {
            //start vs end point. on current reference plane
            CalculateBoxModeGhosts(false);
        }
        else if (_currentDrawMode == DrawMode.boxfill)
        {
            //start vs end point. on current reference plane
            CalculateBoxModeGhosts(true);
        }
        else if (_currentDrawMode == DrawMode.circle)
        {
            //start vs end point. radius or diameter? circle algorithm? ovals?
            CalculateCircleModeGhosts(false);
        }
        else if (_currentDrawMode == DrawMode.circlefill)
        {
            //start vs end point. radius or diameter? circle algorithm? ovals?
            CalculateCircleModeGhosts(true);
        }
    }

    void CalculatePointModeGhost()
    {
        Vector3 intersectionPos = GetCurrentIntersectPoint();
        if (intersectionPos != Vector3.negativeInfinity)
        {
                _ghostDrawPositionList = new List<Vector3Int>();
                _ghostDrawPositionList.Add(Map.GetIntVector3(intersectionPos));
                unHideGhosts();
        }
        else
        {
            hideGhosts();
        }
    }

    void CalculateLineModeGhosts()
    {
        //todo
    }
    
    void CalculateBoxModeGhosts(bool fill)
    {
        Vector3 intersectionPos = GetCurrentIntersectPoint();
        if (intersectionPos != Vector3.negativeInfinity)
        {
            _ghostDrawPositionList = new List<Vector3Int>();
            Vector3Int drawEndPosition = Map.GetIntVector3(intersectionPos);
        
            //since positions will always be on the same plane, one of the axis will have the same value for both
            //so looping will only loop that axis once each time
            //and we don't have to actuually logic which plane we are on
            int xMin = Math.Min(_drawStartPosition.x, drawEndPosition.x);
            int xMax = Math.Max(_drawStartPosition.x, drawEndPosition.x);
            int yMin = Math.Min(_drawStartPosition.y, drawEndPosition.y);
            int yMax = Math.Max(_drawStartPosition.y, drawEndPosition.y);
            int zMin = Math.Min(_drawStartPosition.z, drawEndPosition.z);
            int zMax = Math.Max(_drawStartPosition.z, drawEndPosition.z);
            //todo maybe limit max size if necessary
        
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    for (int z = zMin; z <= zMax; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        //only fill insides if fill enabled
                        if (fill || IsPositionOnBoxEdge(pos, xMin, xMax, yMin, yMax, zMin, zMax))
                        {
                            _ghostDrawPositionList.Add(pos);
                        }
                    }
                }
            }
            unHideGhosts();
        }
        else
        {
            hideGhosts();
        }
        
       
    }

    bool IsPositionOnBoxEdge(Vector3Int pos, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        //one of these will always occur on the aligned axis, so if 2+ occur than it's an edge
        int edgeCollisions = 0;
        if (pos.x == minX || pos.x ==maxX) {edgeCollisions++;}
        if (pos.y == minY || pos.y == maxY) {edgeCollisions++;}
        if (pos.z == minZ || pos.z == maxZ) {edgeCollisions++;}
            
        if(edgeCollisions >= 2){return true;}
        
        return false;
    }
    
    void CalculateCircleModeGhosts(bool fill)
    {
        
    }
    
    #endregion

    //possible performance improvement: render every x frames
    void PlacePlacementIndicatorGhosts()
    {
        if (_ghostSelectedBlock == null) { return; }
        //dynamically resize list, reuse game objects where possible
        //first shrink gameobjects to fit (if list bigger)
        while (_ghostDrawPositionList.Count < _ghostTransformList.Count)
        {
            Transform ghostToExorcise = _ghostTransformList[0];
            _ghostTransformList.RemoveAt(0);
            GameObject.Destroy(ghostToExorcise.gameObject);
        }
        //then reuse existing ghosts
        int i = 0;
        while (i < _ghostTransformList.Count)
        {
            _ghostTransformList[i].position = _ghostDrawPositionList[i];
            i++;
        }
        //finally create new ghosts if needed
        while (_ghostTransformList.Count < _ghostDrawPositionList.Count)
        {
            GameObject newGhost = Instantiate(_ghostSelectedBlock, _ghostParent.transform);
            newGhost.name = "Ghost " + i;
            newGhost.transform.position = _ghostDrawPositionList[i];
            _ghostTransformList.Add(newGhost.transform);
            i++;
        }
    }
    void HandleRotation() 
    {
        //todo rotate each ghost individually

        objectPlacementRotationTimer += Time.deltaTime;
        Quaternion currentRotation = Quaternion.Lerp(startRotation, destRotation,
            objectPlacementRotationTimer / objectPlacementRotationDuration);
        _ghostSelectedBlock.transform.rotation = currentRotation;
        foreach (Transform ghostTransform in _ghostTransformList)
        {
            ghostTransform.rotation = currentRotation;
        }
        if (objectPlacementRotationTimer > objectPlacementRotationDuration)
        {
            isRotating = false;
        }
    }

    List<Command> PlaceProjectedBlocks()
    {
        //pencil places blocks as it goes, TODO figure out how to make these a commandGroup for simple undo
        if (_currentDrawMode == DrawMode.pencil) { return null; }
        
        //depending on draw mode (though i guess all can be grouped, even if its just one block)
        //create command group
        //todo consider refactoring whole solution to use a list of command groups rather than a list of commands
        //todo its basically the same, but perhaps neater
        List<Command> AllCommands = new List<Command>();
        CommandGroup commandGroup = new CommandGroup();
        AllCommands.Add(commandGroup);
        //for each projected block
        foreach (Transform ghostTransform in _ghostTransformList)
        {
            //get place commands and add to command group in order
            List<Command> commands = PlaceCurrentObject(ghostTransform.position);
            foreach (Command command in commands)
            {
                commandGroup.AddCommand(command);
            }
        }
        return AllCommands;
    }
    
    //todo swap to Place ProjectedBlocks
    public List<Command> PlaceCurrentObject(Vector3 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject() || isRotating) { return null; }

        List<Command> commands = new List<Command>();

        if (_currentPlacementType == ObjectType.block && currentBlock != null) { commands = GetPlaceBlockCommands(Map.GetIntVector3(pos), _ghostSelectedBlock.transform.rotation, currentListID, currentBlockBaseID, currentBlockVarientID); }
        else if (_currentPlacementType == ObjectType.wall && currentBlock != null) { commands = GetPlaceWallCommands(pos, _ghostSelectedBlock.transform.rotation, currentListID, currentBlockBaseID, currentBlockVarientID); }
        else if (_currentPlacementType == ObjectType.redPlayer || _currentPlacementType == ObjectType.bluePlayer) { commands = GetPlacePlayerCommand(Map.GetIntVector3(pos), _ghostSelectedBlock.transform.rotation, (_currentPlacementType == ObjectType.redPlayer)); }

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
        ComplexBlock complexBlockBeingPlaced = _ghostSelectedBlock.GetComponent<ComplexBlock>();
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

        void hideGhosts()
    {
        if (_ghostSelectedBlock != null) { _ghostSelectedBlock.SetActive(false); }
        _ghostParent.SetActive(false);
    }

    void unHideGhosts()
    {
        if (_ghostSelectedBlock != null) { _ghostSelectedBlock.SetActive(true); }
        _ghostParent.SetActive(true);
    }
    
    #endregion
    
    #region helper functions
    
    public void SetLevelIntersectionPlane(Vector3 normal, int yLevel)
    {
        levelIntersectionPlane.SetNormalAndPosition(normal, new Vector3(0, yLevel, 0));
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

        startRotation = _ghostSelectedBlock.transform.rotation;
        _ghostSelectedBlock.transform.RotateAround(transform.position, rotationAxis, rotationAmount);
        destRotation = _ghostSelectedBlock.transform.rotation;
        _ghostSelectedBlock.transform.rotation = startRotation;
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

    Vector3 GetCurrentIntersectPoint()
    {
        Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition));
        float distance;
        Vector3 intersectionPos = Vector3.negativeInfinity;
        if (levelIntersectionPlane.Raycast(ray, out distance))
        {
                return ray.GetPoint(distance);
        }
        return Vector3.negativeInfinity;
    }
    
    #endregion
}
