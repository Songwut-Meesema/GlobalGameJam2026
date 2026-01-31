using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "RhythmGame/EnemyTemplate")]
public class EnemyTemplate : ScriptableObject
{
    public string enemyName;
    public float maxHp = 100f;
    public GameObject visualPrefab; 
    public float normalDamage = 10f; 
}
