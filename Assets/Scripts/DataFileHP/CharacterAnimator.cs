using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;
    public bool isPlayer;

    private void OnEnable()
    {
       
        if (isPlayer)
        {
            BattleEvents.OnPlayerAttack += HandlePlayerAttack;
            BattleEvents.OnPlayerHurt += HandlePlayerHurt;
        }
        else
        {
            BattleEvents.OnEnemyAttack += HandleEnemyAttack;
            BattleEvents.OnEnemyHurt += HandleEnemyHurt;
        }
    }

    private void OnDisable()
    {
        
        BattleEvents.OnPlayerAttack -= HandlePlayerAttack;
        BattleEvents.OnPlayerHurt -= HandlePlayerHurt;
        BattleEvents.OnEnemyAttack -= HandleEnemyAttack;
        BattleEvents.OnEnemyHurt -= HandleEnemyHurt;
    }

    // --- Named Methods ---

    private void HandlePlayerAttack(bool isPerfect)
    {
        if (animator == null) return; 
        animator.SetTrigger(isPerfect ? "PerfectAttack" : "Attack");
    }

    private void HandlePlayerHurt()
    {
        if (animator == null) return;
        animator.SetTrigger("Hurt");
    }

    private void HandleEnemyAttack()
    {
        if (animator == null) return;
        animator.SetTrigger("Attack");
    }

    private void HandleEnemyHurt()
    {
        if (animator == null) return;
        animator.SetTrigger("Hurt");
    }
}