using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActionMap;

    [SerializeField]
    string actionMapName;

    InputAction moveAction { get; set; }
    InputAction rotateAction { get; set; }
    InputAction undoAction { get; set; }

    public Vector2 MoveInput { get; private set; }
    public float RotateInput { get; private set; }
    public bool UndoInput { get; private set; }

    void Awake()
    {
        moveAction = inputActionMap.FindActionMap(actionMapName).FindAction("Move");
        rotateAction = inputActionMap.FindActionMap(actionMapName).FindAction("Rotate");
        undoAction = inputActionMap.FindActionMap(actionMapName).FindAction("Undo");
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        rotateAction.performed += context => RotateInput = context.ReadValue<float>();
        rotateAction.canceled += context => RotateInput = 0f;

        undoAction.performed += context => UndoInput = true;
        undoAction.canceled += context => UndoInput = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        rotateAction.Enable();
        undoAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) { moveAction.Disable(); }
        if (rotateAction != null) { rotateAction.Disable(); }
        if (undoAction != null) { undoAction.Disable();}
    }

}
