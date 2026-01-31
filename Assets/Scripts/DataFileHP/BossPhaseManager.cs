using UnityEngine;
using System;

public class BossPhaseManager : MonoBehaviour
{
    public static event Action<int> OnPhaseChanged; 

    [Header("References")]
    [SerializeField] private CharacterStats bossStats;

    [Header("Phase Settings (0.0 - 1.0)")]
    [Range(0, 1)] public float phase2Threshold = 0.7f;
    [Range(0, 1)] public float phase3Threshold = 0.3f;

    private int currentPhase = 1;

    private void OnEnable() {
        if (bossStats != null) bossStats.OnHpChanged += CheckPhase;
    }

    private void OnDisable() {
        if (bossStats != null) bossStats.OnHpChanged -= CheckPhase;
    }

    private void CheckPhase(float currentHp, float maxHp) {
        float hpPercent = currentHp / maxHp;
        int nextPhase = 1;

        if (hpPercent <= phase3Threshold) nextPhase = 3;
        else if (hpPercent <= phase2Threshold) nextPhase = 2;

        if (nextPhase > currentPhase) {
            currentPhase = nextPhase;
            OnPhaseChanged?.Invoke(currentPhase);
            Debug.Log($"<color=red>Phase Switched to: {currentPhase}</color>");
        }
    }
}