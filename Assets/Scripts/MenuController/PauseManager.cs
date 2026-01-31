using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI countdownText;
    public GameObject pausePanel;
    private bool isPaused = false;
    private bool isTransitioning = false;
    [SerializeField] private float fadeDuration = 0.3f; // ปรับความเร็วในการ Fade ตรงนี้
    
    private CanvasGroup canvasGroup;
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
        if (isTransitioning) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (isTransitioning) return;
        StartCoroutine(ResumeCountdown());
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
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        isPaused = true;
        Time.timeScale = 0f; // หยุดเกมทันที

        // แสดงเมาส์
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.PauseSong();

        pausePanel.SetActive(true);
        fadeCoroutine = StartCoroutine(FadePauseMenu(1f, true));
    }

    private IEnumerator ResumeCountdown()
    {
        isTransitioning = true;
        countdownText.gameObject.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 0f;
        pausePanel.SetActive(false);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.ResumeSong();
        isTransitioning = false;
    }

    private IEnumerator FadePauseMenu(float targetAlpha, bool isOpening)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (!isOpening)
        {
            pausePanel.SetActive(false);
        }
    }
}
