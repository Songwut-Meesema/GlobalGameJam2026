using UnityEngine;
using System;

public class BossPhaseManager : MonoBehaviour
{
    public static event Action<int> OnPhaseChanged; 

    [Header("References")]
    [SerializeField] private CharacterStats bossStats;

    [Header("Phase Settings")]
    [SerializeField] private int maxPhases = 3;

    private int currentPhase = 1;

    private void OnEnable() {
        if (bossStats != null) bossStats.OnHpChanged += CheckPhase;
    }

    private void OnDisable() {
        if (bossStats != null) bossStats.OnHpChanged -= CheckPhase;
    }

    private void CheckPhase(float currentHp, float maxHp) {
        if (currentHp <= 0) {
            if (currentPhase < maxPhases) {
                currentPhase++;
                bossStats.ResetHealth(); 
                OnPhaseChanged?.Invoke(currentPhase);
            } 
        }
    }
}