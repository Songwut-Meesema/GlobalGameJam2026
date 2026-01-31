using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; 
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }

        //Change to Input System later
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.UnPause();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.Pause();
    }
}
