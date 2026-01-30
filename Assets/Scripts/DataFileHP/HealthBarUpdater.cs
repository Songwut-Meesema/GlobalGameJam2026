using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUpdater : MonoBehaviour
{
    public Slider slider;
    public CharacterStats stats;

    private void OnEnable() {
        stats.OnHpChanged += UpdateUI;
        UpdateUI(stats.currentHp, stats.maxHp);
    }

    void UpdateUI(float cur, float max) => slider.value = cur / max;
}
