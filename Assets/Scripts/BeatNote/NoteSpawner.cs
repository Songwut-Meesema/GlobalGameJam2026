using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    public SongData songData; 
    public GameObject notePrefab;
    public Transform[] laneSpawnPoints;
    public Transform[] laneHitPoints;
    [SerializeField] public AudioSource musicSource;

    [Header("Settings")]
    public float noteTravelTime = 2f; 
    
    private float currentTravelTime; 
    private int nextIndex = 0;
    private bool musicStarted = false;
    private double dspSongStartTime;

    void Start()
    {
        currentTravelTime = noteTravelTime;
        StartSong();
    }

    private void OnEnable()
    {
        BossPhaseManager.OnPhaseChanged += HandlePhaseSpeed;
    }

    private void OnDisable()
    {
        BossPhaseManager.OnPhaseChanged -= HandlePhaseSpeed;
    }


    public void StartSong()
    {
        if (songData == null || musicSource == null) return;

        if (songData.musicClip != null) musicSource.clip = songData.musicClip;

        //load new clip
        dspSongStartTime = AudioSettings.dspTime + 1.0f; 
        musicSource.PlayScheduled(dspSongStartTime);
        
        if (Conductor.Instance != null)
        {
            Conductor.Instance.dspSongTime = (float)dspSongStartTime;
            Conductor.Instance.songPositionSeconds = 0f;
        }

        musicStarted = true;
        nextIndex = 0; //read new note of first note
        Debug.Log($"<color=green>[Spawner]</color> Song Started: {musicSource.clip.name}");
    }

    public void StopSpawner()
    {
        musicStarted = false;
        if (musicSource != null) musicSource.Stop();
        
        // clear reamining notes
        ClearAllActiveNotes();
        
        Debug.Log("<color=red>[Spawner]</color> Spawner Stopped & Cleared");
    }

    public void RestartSpawnerWithNewData()
    {
       //called by sequencer after changing song data
        StopSpawner();
        StartSong();
    }

    private void ClearAllActiveNotes()
    {
        NoteObject[] notes = Object.FindObjectsOfType<NoteObject>();
        foreach (NoteObject n in notes)
        {
            Destroy(n.gameObject);
        }
    }

    private void HandlePhaseSpeed(int phase)
    {
        // adjust travel time according to phase
        currentTravelTime = noteTravelTime - (phase - 1) * 0.4f;
        currentTravelTime = Mathf.Max(currentTravelTime, 0.8f);
    }

    void Update()
    {
        if (!musicStarted || Conductor.Instance == null || songData == null) return;

        float songTime = Conductor.Instance.songPositionSeconds;

        // check and spawn notes
        while (nextIndex < songData.notes.Count &&
               songData.notes[nextIndex].timeInSeconds <= songTime + currentTravelTime)
        {
            Spawn(songData.notes[nextIndex]);
            nextIndex++;
        }
    }

    void Spawn(NoteInfo info)
    {
        if (info.laneIndex < 0 || info.laneIndex >= laneSpawnPoints.Length) return;

        Transform spawnPoint = laneSpawnPoints[info.laneIndex];
        Transform hitPoint = laneHitPoints[info.laneIndex];

        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        NoteObject noteScript = note.GetComponent<NoteObject>();
        if (noteScript != null)
        {
            noteScript.Initialize(
                info.timeInSeconds, 
                info.laneIndex, 
                spawnPoint, 
                hitPoint, 
                currentTravelTime
            );
        }
    }
}