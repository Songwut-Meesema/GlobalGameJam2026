using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum NoteType
{
    LeftD,
    LeftF,
    RightJ,
    RightK
}


public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private InputAction press;
    public static event Action<NoteType> OnPlayerHit;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.Control.Enable();
        inputActions.Control.LeftD.performed += _ => Emit(NoteType.LeftD);
        inputActions.Control.LeftF.performed += _ => Emit(NoteType.LeftF);
        inputActions.Control.RightJ.performed += _ => Emit(NoteType.RightJ);
        inputActions.Control.RightK.performed += _ => Emit(NoteType.RightK);
    }
    private void OnDisable()
    {
        inputActions.Control.LeftD.performed -= _ => Emit(NoteType.LeftD);
        inputActions.Control.LeftF.performed -= _ => Emit(NoteType.LeftF);
        inputActions.Control.RightJ.performed -= _ => Emit(NoteType.RightJ);
        inputActions.Control.RightK.performed -= _ => Emit(NoteType.RightK);
        inputActions.Control.Disable();
    }
    void Emit(NoteType type)
    {
        Debug.Log("Hit with" + type);
        OnPlayerHit?.Invoke(type);
    }
}
