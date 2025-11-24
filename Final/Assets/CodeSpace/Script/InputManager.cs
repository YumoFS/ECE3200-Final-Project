using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private CodeSpaceInput codeSpaceInput;
    public event EventHandler OnInteractAction;

    private void Awake()
    {
        codeSpaceInput = new CodeSpaceInput();
        codeSpaceInput.Keyboard.Enable();
        codeSpaceInput.Keyboard.Interact.performed += InteractPerformed;
    }
    private void InteractPerformed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementDirection()
    {
        Vector2 inputDirection = codeSpaceInput.Keyboard.Move.ReadValue<Vector2>();
        return inputDirection.normalized;
    }
}
