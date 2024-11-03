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
        //object Placer
        placeAction.performed += context =>
        {
            Command cmd = objectPlacer.PlaceCurrentObject();
            if (cmd != null) { levelEditorController.AddCommand(cmd); }
        };
        RotateForwardAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.FORWARD);
        RotateBackAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.BACK);
        RotateLeftAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.LEFT);
        RotateRightAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.RIGHT);
        RollLeftAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.LEFTROLL);
        RollRightAction.performed += context => objectPlacer.RotateObjectPlacement(ObjectPlacer.RotationDirection.RIGHTROLL);

        undoAction.performed += context => levelEditorController.Undo();
        redoAction.performed += context => levelEditorController.Redo();
        saveAction.performed += context => levelEditorController.Save();
    }

    private void OnEnable()
    {
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
