using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossPhaseSequencer : MonoBehaviour
{
    [Header("Phase Configuration")]
    public List<BossPhaseSettings> phaseSettings; 

    [Header("References")]
    public BossCameraController cameraController;
    public NoteSpawner noteSpawner;
    public AudioSource musicSource;
    public Animator bossAnimator;

    private void OnEnable() => BossPhaseManager.OnPhaseChanged += StartPhaseTransition;
    private void OnDisable() => BossPhaseManager.OnPhaseChanged -= StartPhaseTransition;

    private void StartPhaseTransition(int phaseIndex)
    {
        int settingIndex = phaseIndex - 2; 
        if (settingIndex < 0 || settingIndex >= phaseSettings.Count) return;

        StartCoroutine(ExecuteTransition(phaseSettings[settingIndex], phaseIndex));
    }

    private IEnumerator ExecuteTransition(BossPhaseSettings settings, int phaseNum)
    {
        Debug.Log($"<color=orange>[Sequencer]</color> Starting Phase {phaseNum} Transition");

        noteSpawner.StopSpawner(); 
        musicSource.Stop();
        
        ClearActiveNotes();

        cameraController.ShowBossCamera(phaseNum, settings.showDuration);
        if (bossAnimator != null && !string.IsNullOrEmpty(settings.animationTrigger))
        {
            bossAnimator.SetTrigger(settings.animationTrigger);
        }

        yield return new WaitForSeconds(settings.showDuration);

        Debug.Log("<color=cyan>[Sequencer]</color> Camera returned. Waiting 1.5s before music starts...");
        yield return new WaitForSeconds(1.5f);

        if (settings.phaseSongData != null)
        {
            noteSpawner.songData = settings.phaseSongData;
            musicSource.clip = settings.phaseMusic;
            
            noteSpawner.RestartSpawnerWithNewData();
        }
    }

    private void ClearActiveNotes()
    {
        NoteObject[] activeNotes = FindObjectsOfType<NoteObject>();
        foreach (var note in activeNotes) Destroy(note.gameObject);
    }
}
