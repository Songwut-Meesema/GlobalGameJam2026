using UnityEngine;
using Cinemachine; 

public class CameraJuice : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        NoteEvents.OnNoteHit += HandleHit;
        NoteEvents.OnNotePerfectHit += HandlePerfect;
        NoteEvents.OnNoteMiss += HandleMiss;
    }

    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= HandleHit;
        NoteEvents.OnNotePerfectHit -= HandlePerfect;
        NoteEvents.OnNoteMiss -= HandleMiss;
    }

    private void HandleHit(int _) => StartShake(0.2f);        // เพิ่มค่าความแรง
    private void HandlePerfect(int _) => StartShake(0.5f);    // เพิ่มค่าความแรง
    private void HandleMiss(int _) => StartShake(1.0f);       // เพิ่มค่าความแรง

    public void StartShake(float intensity)
    {
        if (impulseSource == null) return;

        // DEBUG: ถ้ากดแล้ว Log นี้ไม่ขึ้น แสดงว่า Event ไม่มา
        Debug.Log($"<color=yellow>[Juice]</color> Shaking with intensity: {intensity}");

        // สั่งให้สั่นแบบไม่ต้องคำนวณระยะห่าง (สั่นทั้งจอ)
        impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere * intensity);
    }
}