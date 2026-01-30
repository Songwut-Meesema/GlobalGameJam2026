using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "RhythmGame/EnemyTemplate")]
public class EnemyTemplate : ScriptableObject
{
    public string enemyName;
    public float maxHp = 100f;
    public GameObject visualPrefab; // model or sprite for the enemy
    public float normalDamage = 10f; // dmg when u miss a note
}
