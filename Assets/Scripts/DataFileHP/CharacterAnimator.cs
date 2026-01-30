using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;
    public bool isPlayer;

    private void OnEnable() {
        if (isPlayer) {
            BattleEvents.OnPlayerAttack += (isPerfect) => {
                animator.SetTrigger(isPerfect ? "PerfectAttack" : "Attack");
            };
            BattleEvents.OnPlayerHurt += () => animator.SetTrigger("Hurt");
        } else {
            BattleEvents.OnEnemyAttack += () => animator.SetTrigger("Attack");
            BattleEvents.OnEnemyHurt += () => animator.SetTrigger("Hurt");
        }
    }
    private void OnDisable() {
        if (isPlayer) {
            BattleEvents.OnPlayerAttack -= (isPerfect) => animator.SetTrigger(isPerfect ? "PerfectAttack" : "Attack");
            BattleEvents.OnPlayerHurt -= () => animator.SetTrigger("Hurt");
        } else {
            BattleEvents.OnEnemyAttack -= () => animator.SetTrigger("Attack");
            BattleEvents.OnEnemyHurt -= () => animator.SetTrigger("Hurt");
        }
    }
}