using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapEditorInputHandler : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActionMap;

    [Header ("Action Map Names")]
    [SerializeField]
    string ObjectPlacerActionMapName;
    [SerializeField]
    string LevelEditorActionMapName, CameraControlsActionMapName;


    [SerializeField]
    ObjectPlacer objectPlacer;

    [SerializeField]
    LevelEditorController levelEditorController;

    //scene camera
    InputAction cameraMoveAction { get; set; }
    InputAction cameraAimAction { get; set; }
    InputAction cameraEnableAimAction { get; set; }

    public Vector3 cameraMoveInput;
    public Vector2 cameraAimInput;
    public bool cameraAimEnabledInput;

    //ObjectPlacer
    InputAction placeAction { get; set; }
    InputAction RotateForwardAction { get; set; }
    InputAction RotateBackAction { get; set; }
    InputAction RotateLeftAction { get; set; }
    InputAction RotateRightAction { get; set; }
    InputAction RollLeftAction { get; set; }
    InputAction RollRightAction { get; set; }



    //LevelEditorController
    InputAction undoAction { get; set; }
    InputAction redoAction { get; set; }
    InputAction saveAction { get; set; }

    void Awake()
    {
        //camera
        cameraMoveAction = inputActionMap.FindActionMap(CameraControlsActionMapName).FindAction("MoveCamera");
        cameraAimAction = inputActionMap.FindActionMap(CameraControlsActionMapName).FindAction("AimCamera");
        cameraEnableAimAction = inputActionMap.FindActionMap(CameraControlsActionMapName).FindAction("EnableCameraAim");

        //object Placer
        placeAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("PlaceBlock");
        RotateForwardAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RotateObjectForward");
        RotateBackAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RotateObjectBack");
        RotateLeftAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RotateObjectLeft");
        RotateRightAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RotateObjectRight");
        RollLeftAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RollObjectLeft");
        RollRightAction = inputActionMap.FindActionMap(ObjectPlacerActionMapName).FindAction("RollObjectRight");

        //LevelEditorController
        undoAction = inputActionMap.FindActionMap(LevelEditorActionMapName).FindAction("Undo");
        redoAction = inputActionMap.FindActionMap(LevelEditorActionMapName).FindAction("Redo");
        saveAction = inputActionMap.FindActionMap(LevelEditorActionMapName).FindAction("Save");

        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        //camera
        cameraMoveAction.performed += context => cameraMoveInput = context.ReadValue<Vector3>();
        cameraMoveAction.canceled += context => cameraMoveInput = Vector3.zero;

        cameraAimAction.performed += context => cameraAimInput = context.ReadValue<Vector2>();
        cameraAimAction.canceled += context => cameraAimInput = Vector3.zero;

        cameraEnableAimAction.performed += context => cameraAimEnabledInput = true;
        cameraEnableAimAction.canceled += context => cameraAimEnabledInput = false;


        //object Placer
        placeAction.performed += context =>
        {
            List<Command> commands = objectPlacer.PlaceCurrentObject();
            if (commands != null && commands.Count > 0) { levelEditorController.AddCommandTurn(commands); }
        };
        RotateForwardAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.FORWARD);
        RotateBackAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.BACK);
        RotateLeftAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.LEFT);
        RotateRightAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.RIGHT);
        RollLeftAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.LEFTROLL);
        RollRightAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.RIGHTROLL);

        //LevelEditorCOntroller
        undoAction.performed += context => levelEditorController.Undo();
        redoAction.performed += context => levelEditorController.Redo();
        saveAction.performed += context => levelEditorController.Save();
    }

    private void OnEnable()
    {
        //camera
        cameraMoveAction.Enable();
        cameraAimAction.Enable();
        cameraEnableAimAction.Enable();

        //object Placer
        placeAction.Enable();
        RotateForwardAction.Enable();
        RotateBackAction.Enable();
        RotateLeftAction.Enable();
        RotateRightAction.Enable();
        RollLeftAction.Enable();
        RollRightAction.Enable();

        //Level Editor Controller
        undoAction.Enable();
        redoAction.Enable();
        saveAction.Enable();
    }

    private void OnDisable()
    {
        //camera
        cameraMoveAction.Disable();
        cameraAimAction.Disable();
        cameraEnableAimAction.Disable();

        //object Placer
        placeAction.Disable();
        RotateForwardAction.Disable();
        RotateBackAction.Disable();
        RotateLeftAction.Disable();
        RotateRightAction.Disable();
        RollLeftAction.Disable();
        RollRightAction.Disable();

        //Level Editor Controller
        undoAction.Disable();
        redoAction.Disable();
        saveAction.Disable();
    }

}
