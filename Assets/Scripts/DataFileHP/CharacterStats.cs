using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewStats", menuName = "RhythmGame/Stats")]
public class CharacterStats : ScriptableObject
{
    public float maxHp = 100f;
    
    [System.NonSerialized] 
    public float currentHp;

    public Action<float, float> OnHpChanged; 

    public void Initialize() {
        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(float amount) {
        currentHp = Mathf.Max(0, currentHp - amount);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void Heal(float amount) {
        currentHp = Mathf.Min(maxHp, currentHp + amount);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void ResetHealth() {
        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }
}