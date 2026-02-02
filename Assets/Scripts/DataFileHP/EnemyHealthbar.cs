using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider[] phaseSliders; // ใส่หลอดเลือด Phase 1, 2, 3 ตามลำดับ

    [Header("References")]
    public CharacterStats stats;

    private int currentPhaseIndex = 0;

    private void OnEnable()
    {
        if (stats != null) stats.OnHpChanged += UpdateUI;
        BossPhaseManager.OnPhaseChanged += HandlePhaseChange;
        InitializeSliders();
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnHpChanged -= UpdateUI;
        BossPhaseManager.OnPhaseChanged -= HandlePhaseChange;
    }

    private void InitializeSliders()
    {
        currentPhaseIndex = 0;
        for (int i = 0; i < phaseSliders.Length; i++)
        {
            if (phaseSliders[i] != null)
            {
                phaseSliders[i].gameObject.SetActive(true); // โชว์ทุกหลอดตั้งแต่เริ่ม (Stack กัน)
                phaseSliders[i].minValue = 0;
                phaseSliders[i].maxValue = stats.maxHp;
                phaseSliders[i].value = stats.maxHp; // เซ็ตให้เต็มทุกหลอด
            }
        }
    }

    private void HandlePhaseChange(int phase)
    {
        int newIndex = phase - 1; // เช่น Phase 2 คือ index 1
        int prevIndex = newIndex - 1;

        // 1. ซ่อนหลอดเลือดของ Phase ที่เพิ่งจบไปทันที
        if (prevIndex >= 0 && prevIndex < phaseSliders.Length)
        {
            phaseSliders[prevIndex].value = 0;
            phaseSliders[prevIndex].gameObject.SetActive(false);
        }

        // 2. อัปเดตเป้าหมายการ Update เลือดไปที่หลอดถัดไป
        currentPhaseIndex = newIndex;

        // 3. [Game Feel Fix] บังคับให้หลอดปัจจุบัน "โชว์" และ "เต็ม" ทันที
        // ป้องกันอาการ Logic สลับช้ากว่าเฟรมการแสดงผล
        if (currentPhaseIndex < phaseSliders.Length && phaseSliders[currentPhaseIndex] != null)
        {
            phaseSliders[currentPhaseIndex].gameObject.SetActive(true);
            phaseSliders[currentPhaseIndex].maxValue = stats.maxHp;
            phaseSliders[currentPhaseIndex].value = stats.maxHp; 
        }
    }

    void UpdateUI(float cur, float max)
    {
        // [Game Feel Fix] ป้องกันไม่ให้ค่า 0 จากเฟสเก่ามาทำให้หลอดใหม่วูบ
        // ถ้าเลือดเป็น 0 แต่ยังไม่ถึงเฟสสุดท้าย ให้ ignore การอัปเดตนี้ไปก่อน
        if (cur <= 0 && currentPhaseIndex < phaseSliders.Length - 1) return;

        if (currentPhaseIndex >= 0 && currentPhaseIndex < phaseSliders.Length)
        {
            Slider activeSlider = phaseSliders[currentPhaseIndex];
            if (activeSlider != null)
            {
                activeSlider.maxValue = max;
                activeSlider.value = cur;
            }
        }

        // กรณี Phase สุดท้ายเลือดหมด (ชนะ) ให้ซ่อนหลอดสุดท้าย
        if (cur <= 0 && currentPhaseIndex == phaseSliders.Length - 1)
        {
            phaseSliders[currentPhaseIndex].gameObject.SetActive(false);
        }
    }
}