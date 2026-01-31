using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Stats References")]
    public CharacterStats playerStats;
    public CharacterStats currentEnemyStats;

    [Header("Damage Settings")]
    public float damageToEnemyNormal = 10f;
    public float damageToEnemyPerfect = 20f;
    public float damageToPlayer = 15f;

    [Header("UI References")]
    public GameObject winUI; // UI Continue

    private void Awake()
    {
        // ต้อง Reset เลือดทุกครั้งที่เริ่มฉาก
        if (playerStats != null) playerStats.Initialize();
        if (currentEnemyStats != null) currentEnemyStats.Initialize();
    }

    private void OnEnable()
    {
        // Subscribe to your NoteObject Static Events
        NoteObject.OnNoteHit += HandleNoteHit;
        NoteObject.OnNoteHitPerfect += HandleNoteHitPerfect;
        NoteObject.OnNoteMiss += HandleNoteMiss;
    }

    private void OnDisable()
    {
        NoteObject.OnNoteHit -= HandleNoteHit;
        NoteObject.OnNoteHitPerfect -= HandleNoteHitPerfect;
        NoteObject.OnNoteMiss -= HandleNoteMiss;
    }

    private void HandleNoteHit(int lane)
    {
        currentEnemyStats.TakeDamage(damageToEnemyNormal);
        BattleEvents.TriggerPlayerAttack(false); // Normal Attack
        BattleEvents.TriggerEnemyHurt();
        CheckEnemyDeath();
    }

    private void HandleNoteHitPerfect(int lane)
    {
        currentEnemyStats.TakeDamage(damageToEnemyPerfect);
        BattleEvents.TriggerPlayerAttack(true); // Perfect Attack
        BattleEvents.TriggerEnemyHurt();
        CheckEnemyDeath();
    }

    private void HandleNoteMiss(int lane)
    {
        Debug.Log($"Player currently has {playerStats.currentHp} HP before taking damage.");
        playerStats.TakeDamage(damageToPlayer);
        Debug.Log($"Damaged Player for {damageToPlayer} HP");
        Debug.Log($"Player currently has {playerStats.currentHp} HP after taking damage.");

        BattleEvents.TriggerPlayerHurt();
        BattleEvents.TriggerEnemyAttack();

        CheckPlayerDeath();
    }

    private void CheckEnemyDeath()
    {
        if (currentEnemyStats.currentHp <= 0)
        {
            winUI.SetActive(true);
            // หยุดเพลงหรือทำอย่างอื่นตามต้องการ
        }
    }

    private void CheckPlayerDeath()
    {
        if (playerStats.currentHp <= 0)
        {
            Debug.Log("Game Over");
            winUI.SetActive(true);
            // ใส่ UI Lose ตรงนี้
        }
    }
}

// Internal Event สำหรับสื่อสารกับ Animator/Visual
public static class BattleEvents {
    public static System.Action<bool> OnPlayerAttack; // true = perfect
    public static System.Action OnPlayerHurt;
    public static System.Action OnEnemyAttack;
    public static System.Action OnEnemyHurt;

    public static void TriggerPlayerAttack(bool p) => OnPlayerAttack?.Invoke(p);
    public static void TriggerPlayerHurt() => OnPlayerHurt?.Invoke();
    public static void TriggerEnemyAttack() => OnEnemyAttack?.Invoke();
    public static void TriggerEnemyHurt() => OnEnemyHurt?.Invoke();
}
