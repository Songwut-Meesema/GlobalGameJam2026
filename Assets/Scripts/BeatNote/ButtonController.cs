using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public static event Action<int> OnPlayerHit;

    private void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        if (inputActions == null) inputActions = new PlayerInputActions();

        inputActions.Control.Enable();
        
        inputActions.Control.LeftD.performed += OnLeftD;
        inputActions.Control.LeftF.performed += OnLeftF;
        inputActions.Control.RightJ.performed += OnRightJ;
        inputActions.Control.RightK.performed += OnRightK;
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Control.LeftD.performed -= OnLeftD;
            inputActions.Control.LeftF.performed -= OnLeftF;
            inputActions.Control.RightJ.performed -= OnRightJ;
            inputActions.Control.RightK.performed -= OnRightK;
            
            inputActions.Control.Disable();
        }
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Dispose();
            inputActions = null;
        }
    }

    private void Emit(int lane)
    {
        if (Conductor.Instance == null) return;
        
        OnPlayerHit?.Invoke(lane);
    }

    private void OnLeftD(InputAction.CallbackContext ctx) => Emit(0);
    private void OnLeftF(InputAction.CallbackContext ctx) => Emit(1);
    private void OnRightJ(InputAction.CallbackContext ctx) => Emit(2);
    private void OnRightK(InputAction.CallbackContext ctx) => Emit(3);
}