using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public struct ComboThreshold
{
    public int minCombo;
    public string badgeName;
}

public class ComboManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] TextMeshProUGUI badgeText;
    [Header("Combo Settings")]
    [SerializeField] List<ComboThreshold> thresholds;
    private int currentCombo = 0;
    private bool isNoteMissed = false;
    private void OnEnable()
    {
        NoteEvents.OnNoteHit += HandleHit;
        NoteEvents.OnNotePerfectHit += HandleHit; 
        NoteEvents.OnNoteMiss += HandleNoteMiss;
    }

    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= HandleHit;
        NoteEvents.OnNotePerfectHit -= HandleHit;
        NoteEvents.OnNoteMiss -= HandleNoteMiss;
    }

    private void HandleHit(int lane)
    {
        currentCombo++;
        UpdateComboUI();
    }

    private void HandleNoteMiss(int lane)
    {
        currentCombo = 0;
        UpdateComboUI();
    }

    private void UpdateComboUI()
    {
        if (currentCombo <= 0)
        {
            if (badgeText) badgeText.text = "";
            return;
        }

        ComboThreshold currentBadge = thresholds
            .OrderByDescending(t => t.minCombo)
            .FirstOrDefault(t => currentCombo >= t.minCombo);

        if (currentBadge.badgeName != null)
        {
            if (badgeText)
            {
                badgeText.text = currentBadge.badgeName;
            }
        }
    }
}
