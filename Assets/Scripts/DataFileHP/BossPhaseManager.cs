using UnityEngine;
using System;
using System.Collections;

public class BossPhaseManager : MonoBehaviour
{
    public static event Action<int> OnPhaseChanged; 

    [Header("References")]
    [SerializeField] private CharacterStats bossStats;

    [Header("Phase Settings")]
    [SerializeField] private int maxPhases = 3;

    private int currentPhase = 1;
    private bool isTransitioning = false;

    private void OnEnable() {
        if (bossStats != null) bossStats.OnHpChanged += CheckPhase;
    }

    private void OnDisable() {
        if (bossStats != null) bossStats.OnHpChanged -= CheckPhase;
    }

    private void CheckPhase(float currentHp, float maxHp) {
        // ถ้าเลือดหมด และยังมีเฟสต่อไป และไม่ได้กำลังเปลี่ยนเฟสอยู่
        if (currentHp <= 0 && currentPhase < maxPhases && !isTransitioning) {
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine() {
        isTransitioning = true;

        // 1. รีเซ็ตเลือดหลังบ้านให้เต็มทันที (Data Reset)
        if (bossStats != null) bossStats.ResetHealth();

        // 2. รอ 1 เฟรม เพื่อให้ Event OnHpChanged (ค่า 0) ของเฟสเก่าทำงานจบก่อน
        yield return null;

        // 3. เปลี่ยนเฟส และแจ้ง UI ให้สลับหลอด
        currentPhase++;
        OnPhaseChanged?.Invoke(currentPhase);

        isTransitioning = false;
        Debug.Log($"<color=cyan>[Phase]</color> Switched to Phase {currentPhase}");
    }
}