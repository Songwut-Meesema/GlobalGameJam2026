using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public static event Action<int> OnPlayerHit;

    private void Awake()
    {
        // สร้าง Instance ครั้งเดียว
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // ตรวจสอบความปลอดภัยก่อน Enable
        if (inputActions == null) inputActions = new PlayerInputActions();

        inputActions.Control.Enable();
        
        // สมัคร Event
        inputActions.Control.LeftD.performed += OnLeftD;
        inputActions.Control.LeftF.performed += OnLeftF;
        inputActions.Control.RightJ.performed += OnRightJ;
        inputActions.Control.RightK.performed += OnRightK;
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            // สำคัญมาก: ต้องถอนการสมัครให้หมดเพื่อไม่ให้เกิด MissingReference
            inputActions.Control.LeftD.performed -= OnLeftD;
            inputActions.Control.LeftF.performed -= OnLeftF;
            inputActions.Control.RightJ.performed -= OnRightJ;
            inputActions.Control.RightK.performed -= OnRightK;
            
            inputActions.Control.Disable();
        }
    }

    private void OnDestroy()
    {
        // คืนหน่วยความจำและล้างระบบ Input เมื่อ Object นี้หายไปจากเกมจริง ๆ
        if (inputActions != null)
        {
            inputActions.Dispose();
            inputActions = null;
        }
    }

    private void Emit(int lane)
    {
        // เช็คว่า Conductor พร้อมไหม (ป้องกัน Error เวลาโหลดซีน)
        if (Conductor.Instance == null) return;
        
        OnPlayerHit?.Invoke(lane);
    }

    private void OnLeftD(InputAction.CallbackContext ctx) => Emit(0);
    private void OnLeftF(InputAction.CallbackContext ctx) => Emit(1);
    private void OnRightJ(InputAction.CallbackContext ctx) => Emit(2);
    private void OnRightK(InputAction.CallbackContext ctx) => Emit(3);
}