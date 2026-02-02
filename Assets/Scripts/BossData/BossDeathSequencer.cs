using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossDeathSequencer : MonoBehaviour
{
    [Header("References")]
    public BossCameraController cameraController;
    public NoteSpawner noteSpawner;
    public Animator bossAnimator;
    public AudioSource musicSource; 
    public CanvasGroup fadePanel;   

    [Header("Settings")]
    [Tooltip("เวลาที่จะรอเพิ่มหลังจาก Animation ตายเล่นจบแล้ว (วินาที)")]
    public float delayAfterAnim = 0.5f; 
    public float fadeDuration = 1.0f;
    public string endSceneName = "EndScene";

    private void OnEnable() 
    {
        BattleEvents.OnEnemyDeath += StartDeathSequence;
    }

    private void OnDisable() 
    {
        BattleEvents.OnEnemyDeath -= StartDeathSequence;
    }

    private void StartDeathSequence()
    {
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        Debug.Log("<color=red>[Death]</color> Boss Defeated! Sequence Started.");

        // 1. หยุดเกมเพลย์
        if (noteSpawner != null) noteSpawner.StopSpawner();
        if (musicSource != null) musicSource.Stop();

        // 2. สั่งกล้องตาย (รหัส 4) ให้จับภาพยาวๆ ไปเลย (20วิ) เดี๋ยวเราตัด Scene ก่อนอยู่แล้ว
        if (cameraController != null) 
        {
            cameraController.ShowBossCamera(4, 20f); 
        }

        // 3. สั่ง Animation ตาย และคำนวณเวลา
        float animDuration = 2.0f; // ค่ากันเหนียวเผื่อหาไม่เจอ

        if (bossAnimator != null) 
        {
            bossAnimator.SetTrigger("Die");

            // สำคัญ! รอ 1 เฟรมเพื่อให้ Animator เปลี่ยน State ทัน
            yield return null; 
            yield return new WaitForSeconds(0.1f); 

            // ดึงความยาวของ Animation ที่กำลังเล่นอยู่ (Layer 0)
            AnimatorStateInfo info = bossAnimator.GetCurrentAnimatorStateInfo(0);
            animDuration = info.length;
            
            Debug.Log($"Animation Length is: {animDuration} seconds");
        }

        // 4. รอจน Animation จบ (ลบ 0.1 ที่รอไปตะกี้)
        yield return new WaitForSeconds(animDuration - 0.1f);

        // 5. รอเพิ่มอีก 0.5 วินาที (หรือตามที่ตั้งไว้ใน delayAfterAnim)
        yield return new WaitForSeconds(delayAfterAnim);

        // 6. เริ่ม Fade ดำ
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.alpha = timer / fadeDuration;
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        // 7. เปลี่ยน Scene
        Debug.Log("Loading End Scene...");
        SceneManager.LoadScene(endSceneName);
    }
}