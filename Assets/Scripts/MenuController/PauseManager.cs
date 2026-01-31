using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField] private float fadeDuration = 0.3f; // ปรับความเร็วในการ Fade ตรงนี้
    
    private CanvasGroup canvasGroup;
    private bool isPaused = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // ตรวจสอบและดึง CanvasGroup (ถ้าไม่มีให้แอดเพิ่มอัตโนมัติ)
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = pausePanel.AddComponent<CanvasGroup>();
        
        // เริ่มต้นด้วยการซ่อน UI
        canvasGroup.alpha = 0f;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        isPaused = false;
        Time.timeScale = 1f; // กลับมาเดินเวลาปกติทันที

        // ซ่อนเมาส์
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.UnPause();

        // Fade Out UI ออกไป
        fadeCoroutine = StartCoroutine(FadePauseMenu(0f, false));
    }

    public void Pause()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        isPaused = true;
        Time.timeScale = 0f; // หยุดเกมทันที

        // แสดงเมาส์
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.Pause();

        // Fade In UI ขึ้นมา
        pausePanel.SetActive(true);
        fadeCoroutine = StartCoroutine(FadePauseMenu(1f, true));
    }

    private IEnumerator FadePauseMenu(float targetAlpha, bool isOpening)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        // สำคัญ: ใช้ Time.unscaledDeltaTime เพื่อให้ทำงานได้ตอน Time.timeScale = 0
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // ถ้าเป็นการ Resume และ Fade Out จนจบแล้ว ให้ปิด Object ไปเลย
        if (!isOpening)
        {
            pausePanel.SetActive(false);
        }
    }
}