using System;
using System.Collections;
using UnityEngine;

public enum gameState
{
    WIN,
    LOSE,
}

public class BattleManager : MonoBehaviour
{
    [Header("Stats References")]
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private CharacterStats currentEnemyStats;

    [Header("Damage Settings")]
    [SerializeField] private float damageToEnemyNormal = 10f;
    [SerializeField] private float damageToEnemyPerfect = 20f;
    [SerializeField] private float damageNoteToPlayer = 15f;
    [SerializeField] private float damageBombToPlayer = 30f;

    [Header("UI References")]
    // [SerializeField] private GameObject winUI; 
    [SerializeField] private GameObject loseUI;

    public static event Action<gameState> OnGameStateChanged;

    private bool isGameOver = false;

    private void Awake()
    {
        isGameOver = false;
        if (playerStats != null) playerStats.Initialize();
        if (currentEnemyStats != null) currentEnemyStats.Initialize();
    }

    private void Start()
    {
        // ปิด UI ทั้งหมดตอนเริ่ม
        // if (winUI != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe to your NoteObject Static Events
        NoteEvents.OnNoteHit += HandleNoteHit;
        NoteEvents.OnNotePerfectHit += HandleNoteHitPerfect;
        NoteEvents.OnNoteMiss += HandleNoteMiss;
        NoteEvents.OnBombHit += HandleBombHit;
        NoteEvents.OnHealingNoteHit += HandleHealingNoteHit;
        NoteEvents.OnNoteEarlyHit += HandleNoteMiss;
    }

    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= HandleNoteHit;
        NoteEvents.OnNotePerfectHit -= HandleNoteHitPerfect;
        NoteEvents.OnNoteMiss -= HandleNoteMiss;
        NoteEvents.OnBombHit -= HandleBombHit;
        NoteEvents.OnHealingNoteHit -= HandleHealingNoteHit;
        NoteEvents.OnNoteEarlyHit -= HandleNoteMiss;
    }

    private void HandleNoteHit(int lane)
    {
        if (isGameOver) return;

        currentEnemyStats.TakeDamage(damageToEnemyNormal);
        BattleEvents.TriggerPlayerAttack(false);
        BattleEvents.TriggerEnemyHurt();
        CheckEnemyDeath();
    }

    private void HandleNoteHitPerfect(int lane)
    {
        if (isGameOver) return;

        currentEnemyStats.TakeDamage(damageToEnemyPerfect);
        BattleEvents.TriggerPlayerAttack(true);
        BattleEvents.TriggerEnemyHurt();
        CheckEnemyDeath();
    }

    private void HandleNoteMiss(int lane)
    {
        if (isGameOver) return;

        playerStats.TakeDamage(damageNoteToPlayer);
        BattleEvents.TriggerPlayerHurt();
        BattleEvents.TriggerEnemyAttack();
        CheckPlayerDeath();
    }

    private void HandleBombHit(int lane)
    {
        if (isGameOver) return;

        playerStats.TakeDamage(damageBombToPlayer);
        BattleEvents.TriggerPlayerHurt();
        BattleEvents.TriggerEnemyAttack();
        CheckPlayerDeath();
    }

    private void HandleHealingNoteHit(int lane, float amount)
    {
        if (isGameOver) return;

        playerStats.Heal(amount);
    }

    private void CheckEnemyDeath()
    {
        if (currentEnemyStats.currentHp <= 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("<color=green>[BATTLE] Victory!</color>");
            // if (winUI != null) winUI.SetActive(true);
            OnGameStateChanged?.Invoke(gameState.WIN);
            BattleEvents.TriggerEnemyDeath();
            // stio music when win
            StopMusic();
        }
    }

    private void CheckPlayerDeath()
    {
        if (playerStats.currentHp <= 0 && !isGameOver)
        {
            // เช็คกรณีพิเศษ: ถ้าบอสตายพร้อมเรา (Double KO) ให้ถือว่าเราชนะ
            if (currentEnemyStats.currentHp <= 0)
            {
                CheckEnemyDeath(); // โยนไปเข้าฟังก์ชันชนะแทน
                return;
            }

            // ถ้าบอสไม่ตาย แสดงว่าเราแพ้จริง
            isGameOver = true;
            Debug.Log("<color=red>[BATTLE] Defeat!</color>");

            // 1. แจ้ง GameState ว่าแพ้
            OnGameStateChanged?.Invoke(gameState.LOSE);
            
            // 2. เปิดหน้าต่างแพ้
            if (loseUI != null) loseUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            StopMusic();
        }
    }

    private void StopMusic()
    {
        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
        {
            Conductor.Instance.musicSource.Stop();
        }
    }

}

public static class BattleEvents
{
    public static System.Action<bool> OnPlayerAttack;
    public static System.Action OnPlayerHurt;
    public static System.Action OnEnemyAttack;
    public static System.Action OnEnemyHurt;

    public static void TriggerPlayerAttack(bool p) => OnPlayerAttack?.Invoke(p);
    public static void TriggerPlayerHurt() => OnPlayerHurt?.Invoke();
    public static void TriggerEnemyAttack() => OnEnemyAttack?.Invoke();
    public static void TriggerEnemyHurt() => OnEnemyHurt?.Invoke();

    public static System.Action OnEnemyDeath;
    public static void TriggerEnemyDeath() => OnEnemyDeath?.Invoke();
}