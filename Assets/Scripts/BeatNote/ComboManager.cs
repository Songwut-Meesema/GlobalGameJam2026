using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ComboThreshold
{
    public int minCombo;
    public Sprite badgeSprite;
}

public class ComboManager : MonoBehaviour
{
    [SerializeField] Image badgeDisplay;
    [Header("Combo Settings")]
    [SerializeField] List<ComboThreshold> thresholds;
    private int currentCombo = 0;
    
    private void Start()
    {
        if (badgeDisplay)
        {
            badgeDisplay.gameObject.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        NoteEvents.OnNoteHit += HandleHit;
        NoteEvents.OnNotePerfectHit += HandleHit; 
        NoteEvents.OnNoteMiss += HandleNoteMiss;
        NoteEvents.OnNoteEarlyHit += HandleNoteMiss;
        NoteEvents.OnBombHit += HandleNoteMiss;
    }

    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= HandleHit;
        NoteEvents.OnNotePerfectHit -= HandleHit;
        NoteEvents.OnNoteMiss -= HandleNoteMiss;
        NoteEvents.OnNoteEarlyHit += HandleNoteMiss;
        NoteEvents.OnBombHit += HandleNoteMiss;
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
        ShowMissBadge();
    }

    private void ShowMissBadge()
    {
        if (badgeDisplay)
        {
            badgeDisplay.gameObject.SetActive(true);
            ComboThreshold missBadge = thresholds.FirstOrDefault(t => t.minCombo == 0);
            if (missBadge.badgeSprite != null)
            {
                badgeDisplay.sprite = missBadge.badgeSprite;
            }
        }
    }

    private void UpdateComboUI()
    {
        if (currentCombo <= 0)
        {
            if (badgeDisplay) 
            {
                badgeDisplay.sprite = null;
                badgeDisplay.gameObject.SetActive(false);
            }
            return;
        }

        if (badgeDisplay && !badgeDisplay.gameObject.activeSelf)
        {
            badgeDisplay.gameObject.SetActive(true);
        }

        ComboThreshold currentBadge = thresholds
            .OrderByDescending(t => t.minCombo)
            .FirstOrDefault(t => currentCombo >= t.minCombo);

        if (currentBadge.badgeSprite != null)
        {
            if (badgeDisplay)
            {
                badgeDisplay.sprite = currentBadge.badgeSprite;
            }
        }
    }
}