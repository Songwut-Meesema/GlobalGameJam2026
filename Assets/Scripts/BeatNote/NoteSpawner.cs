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
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    public float noteTravelTime = 2f; 
    
    private float currentTravelTime; // speed for current phase
    private int nextIndex = 0;
    private bool musicStarted = false;
    private double dspSongStartTime;

    void Start()
    {
        // เริ่มต้นด้วยความเร็วพื้นฐาน
        currentTravelTime = noteTravelTime;
        StartSong();
    }

    private void OnEnable()
    {
        //listen to phase change to adjust note speed
        BossPhaseManager.OnPhaseChanged += HandlePhaseSpeed;
    }

    private void OnDisable()
    {
        BossPhaseManager.OnPhaseChanged -= HandlePhaseSpeed;
    }

    public void StartSong()
    {
        //time dilation for audio sync accuracy
        dspSongStartTime = AudioSettings.dspTime + 1.0f; 
        musicSource.PlayScheduled(dspSongStartTime);
        musicStarted = true;
    }

    private void HandlePhaseSpeed(int phase)
    {
        //higher phase = travel time less
        // Phase 1: 2.0s | Phase 2: 1.6s | Phase 3: 1.2s (Examlple)
        currentTravelTime = noteTravelTime - (phase - 1) * 0.4f;
        
        // ป้องกันไม่ให้เร็วเกินไปจนกดไม่ได้ (ต่ำสุด 0.8 วินาที)
        currentTravelTime = Mathf.Max(currentTravelTime, 0.8f);

        Debug.Log($"<color=cyan>[Spawner]</color> Speed Updated: {currentTravelTime}s");
    }

    void Update()
    {
        if (!musicStarted || Conductor.Instance == null) return;

        float songTime = Conductor.Instance.songPositionSeconds;

      
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