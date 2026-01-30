using UnityEngine;
using UnityEngine.UI;

public class HealthBarUpdater : MonoBehaviour
{
    public Slider slider;
    public CharacterStats stats;

    private void OnEnable() 
    {
        if (stats != null) stats.OnHpChanged += UpdateUI;
    }

    private void OnDisable()
     {
        if (stats != null) stats.OnHpChanged -= UpdateUI;
    }

    private void Start() 
    {
        if (stats != null) 
        {
            float initialHp = stats.currentHp > 0 ? stats.currentHp : stats.maxHp;
            UpdateUI(initialHp, stats.maxHp);
        }
    }

    void UpdateUI(float cur, float max
    ) {
        if (slider != null && max > 0)
         {
            slider.value = cur / max;
        }
    }
}