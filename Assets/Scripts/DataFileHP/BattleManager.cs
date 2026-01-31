using UnityEngine;

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
    [SerializeField] private GameObject winUI;  
    [SerializeField] private GameObject loseUI; 

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
        if (winUI != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe to your NoteObject Static Events
        NoteEvents.OnNoteHit += HandleNoteHit;
        NoteEvents.OnNotePerfectHit += HandleNoteHitPerfect;
        NoteEvents.OnNoteMiss += HandleNoteMiss;
        NoteEvents.OnBombHit += HandleBombHit;
    }

    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= HandleNoteHit;
        NoteEvents.OnNotePerfectHit -= HandleNoteHitPerfect;
        NoteEvents.OnNoteMiss -= HandleNoteMiss;
        NoteEvents.OnBombHit -= HandleBombHit;

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

    private void CheckEnemyDeath()
    {
        if (currentEnemyStats.currentHp <= 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("<color=green>[BATTLE] Victory!</color>");
            if (winUI != null) winUI.SetActive(true);
            
            // stio music when win
            StopMusic();
        }
    }

    private void CheckPlayerDeath()
    {
        if (playerStats.currentHp <= 0 && !isGameOver)
        {
            isGameOver = true;
            // Debug.Log("<color=red>[BATTLE] Player Defeated!</color>");
            if (loseUI != null) loseUI.SetActive(true);
            
            // stop music when lose
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

public static class BattleEvents {
    public static System.Action<bool> OnPlayerAttack; 
    public static System.Action OnPlayerHurt;
    public static System.Action OnEnemyAttack;
    public static System.Action OnEnemyHurt;

    public static void TriggerPlayerAttack(bool p) => OnPlayerAttack?.Invoke(p);
    public static void TriggerPlayerHurt() => OnPlayerHurt?.Invoke();
    public static void TriggerEnemyAttack() => OnEnemyAttack?.Invoke();
    public static void TriggerEnemyHurt() => OnEnemyHurt?.Invoke();
}