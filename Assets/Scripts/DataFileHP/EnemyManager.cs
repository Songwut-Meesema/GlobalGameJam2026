using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyTemplate> enemyList;
    private int currentIndex = 0;
    
    public Transform enemySpawnPoint;
    public CharacterStats enemyStatsAsset;
    public GameObject winUI;

    void Start() {
        SpawnNextEnemy();
    }

    public void SpawnNextEnemy() {
        if (currentIndex >= enemyList.Count) return;

        winUI.SetActive(false);
        
        // ลบตัวเก่า
        foreach (Transform child in enemySpawnPoint) Destroy(child.gameObject);

        // ตั้งค่าตัวใหม่
        EnemyTemplate et = enemyList[currentIndex];
        enemyStatsAsset.maxHp = et.maxHp;
        enemyStatsAsset.Initialize();

        GameObject enemyObj = Instantiate(et.visualPrefab, enemySpawnPoint);
        // ใส่สคริปต์ Animator ให้ศัตรูตัวที่เสกออกมา
        enemyObj.AddComponent<CharacterAnimator>().isPlayer = false;

        currentIndex++;
    }
}
