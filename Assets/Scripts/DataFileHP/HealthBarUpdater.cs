using UnityEngine;
using UnityEngine.UI;

public class HealthBarUpdater : MonoBehaviour
{
    public Slider slider;
    public CharacterStats stats;

    private void OnEnable()
    {
        if (stats != null) 
        {
            stats.OnHpChanged += UpdateUI;
            // ดึงค่าปัจจุบันมาแสดงทันที
            UpdateUI(stats.currentHp, stats.maxHp);
        }
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnHpChanged -= UpdateUI;
    }

    void UpdateUI(float cur, float max)
    {
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = max; 
            slider.value = cur; 

            Debug.Log($"HP Update: {cur}/{max} -> Slider Value: {slider.value}");
        }
    }
}