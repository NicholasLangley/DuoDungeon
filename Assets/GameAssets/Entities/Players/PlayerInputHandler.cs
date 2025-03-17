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
    InputAction aimAction { get; set; }
    InputAction rotateAction { get; set; }
    InputAction undoAction { get; set; }
    InputAction attackAction { get; set; }
    InputAction interactAction { get; set; }

    public Vector2 moveInput { get; private set; }
    public Vector2 aimInput { get; private set; }
    public float rotateInput { get; private set; }
    public bool undoInput { get; private set; }
    public bool attackInput { get; private set; }
    public bool interactInput { get; private set; }


    void Awake()
    {
        moveAction = inputActionMap.FindActionMap(actionMapName).FindAction("Move");
        aimAction = inputActionMap.FindActionMap(actionMapName).FindAction("Aim");
        rotateAction = inputActionMap.FindActionMap(actionMapName).FindAction("Rotate");
        undoAction = inputActionMap.FindActionMap(actionMapName).FindAction("Undo");
        attackAction = inputActionMap.FindActionMap(actionMapName).FindAction("Attack");
        interactAction = inputActionMap.FindActionMap(actionMapName).FindAction("Interact");
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        aimAction.performed += context => aimInput = context.ReadValue<Vector2>();
        aimAction.canceled += context => aimInput = Vector2.zero;

        rotateAction.performed += context => rotateInput = context.ReadValue<float>();
        rotateAction.canceled += context => rotateInput = 0f;

        undoAction.performed += context => undoInput = true;
        undoAction.canceled += context => undoInput = false;

        attackAction.performed += context => attackInput = true;
        attackAction.canceled += context => attackInput = false;

        interactAction.performed += context => interactInput = true;
        interactAction.canceled += context => interactInput = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        aimAction.Enable();
        rotateAction.Enable();
        undoAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        aimAction.Disable();
        rotateAction.Disable();
        undoAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
    }

}
