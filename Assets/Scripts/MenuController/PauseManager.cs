using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI countdownText;
    public GameObject pausePanel;
    private bool isPaused = false;
    private bool isTransitioning = false;

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
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.PauseSong();
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

}
