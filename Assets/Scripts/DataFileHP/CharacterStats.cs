using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewStats", menuName = "RhythmGame/Stats")]
public class CharacterStats : ScriptableObject
{
    public float maxHp = 100f;
    public float currentHp;

    // Event triggered when HP changes: parameters are current HP and max HP
    public Action<float, float> OnHpChanged; 

    public void Initialize() {
        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(float amount) {
        currentHp = Mathf.Max(0, currentHp - amount);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }
}