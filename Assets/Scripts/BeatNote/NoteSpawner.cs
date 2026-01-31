using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotePrefabData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float percentageOfAppearing;
}

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    public SongData songData;
    public NotePrefabData[] notePrefab;
    public Transform[] laneSpawnPoints;
    public Transform[] laneHitPoints;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float delayDurationBeforeNextPhase = 5f;

    [Header("Settings")]
    [SerializeField] private int maxBombsPerMoment = 1;
    public float noteTravelTime = 2f;
    private float currentTravelTime;
    private int nextIndex = 0;
    private bool musicStarted = false;
    private double dspSongStartTime;
    private int[] weightTable = new int[100];
    private float lastSpawnTime = -1f;
    private int bombsSpawnedThisMoment = 0;

    void Start()
    {
        currentTravelTime = noteTravelTime;
        GenerateLookupTable();
        StartSong();
    }

    private void OnEnable()
    {
        //listen to phase change to adjust note speed
        BossPhaseManager.OnPhaseChanged += HandlePhaseChange;
    }

    private void OnDisable()
    {
        BossPhaseManager.OnPhaseChanged -= HandlePhaseChange;
    }


    public void StartSong()
    {
        if (songData == null || musicSource == null) return;

        if (songData.musicClip != null) musicSource.clip = songData.musicClip;
        //time dilation for audio sync accuracy
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
        NoteObject[] notes = UnityEngine.Object.FindObjectsOfType<NoteObject>();
        foreach (NoteObject n in notes)
        {
            Destroy(n.gameObject);
        }
    }

    private void HandlePhaseChange(int phase)
    {

        currentTravelTime = noteTravelTime - (phase - 1) * 0.4f;

        currentTravelTime = Mathf.Max(currentTravelTime, 0.8f);

        if (phase == 2)
        {
            SetNoteProbability<NoteBomb>(20f);
        }
        else if (phase == 3)
        {
            SetNoteProbability<NoteBomb>(40f);
        }
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

        if (!Mathf.Approximately(info.timeInSeconds, lastSpawnTime))
        {
            lastSpawnTime = info.timeInSeconds;
            bombsSpawnedThisMoment = 0;
        }

        Transform spawnPoint = laneSpawnPoints[info.laneIndex];
        Transform hitPoint = laneHitPoints[info.laneIndex];

        GameObject noteObject = GetWeightedNotePrefab();

        if (noteObject.GetComponent<NoteBomb>() != null && bombsSpawnedThisMoment >= maxBombsPerMoment)
        {
            noteObject = GetNonBombPrefab();
        }

        if (noteObject.GetComponent<NoteBomb>() != null)
            bombsSpawnedThisMoment++;

        GameObject note = Instantiate(noteObject, spawnPoint.position, Quaternion.Euler(0, 90, 0));

        Notebase noteScript = note.GetComponent<Notebase>();
        noteScript.Initialize(info.timeInSeconds, info.laneIndex, spawnPoint, hitPoint, noteTravelTime);
    }

    GameObject GetWeightedNotePrefab()
    {
        return notePrefab[weightTable[UnityEngine.Random.Range(0, 100)]].prefab;
    }

    GameObject GetNonBombPrefab()
    {
        List<GameObject> safeNotes = new List<GameObject>();

        foreach (var n in notePrefab)
        {
            if (n.prefab.GetComponent<NoteBomb>() == null)
                safeNotes.Add(n.prefab);
        }

        return safeNotes[UnityEngine.Random.Range(0, safeNotes.Count)];
    }


    void GenerateLookupTable()
    {
        int currentSlot = 0;
        for (int i = 0; i < notePrefab.Length; i++)
        {
            int slotsToFill = Mathf.RoundToInt(notePrefab[i].percentageOfAppearing);
            for (int j = 0; j < slotsToFill && currentSlot < 100; j++)
            {
                weightTable[currentSlot] = i;
                currentSlot++;
            }
        }
        while (currentSlot < 100)
        {
            weightTable[currentSlot] = 0;
            currentSlot++;
        }
    }

    private void SetNoteProbability<T>(float targetPercent) where T : MonoBehaviour
    {
        int targetIndex = -1;
        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (notePrefab[i].prefab.GetComponent<T>() != null)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return;

        notePrefab[targetIndex].percentageOfAppearing = targetPercent;

        NormalizeOtherNotes(targetIndex, targetPercent);
        GenerateLookupTable();
    }

    private void NormalizeOtherNotes(int fixedIndex, float fixedPercent)
    {
        float remainingPercent = 100f - fixedPercent;
        float currentOtherTotal = 0f;

        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (i != fixedIndex) currentOtherTotal += notePrefab[i].percentageOfAppearing;
        }

        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (i != fixedIndex && currentOtherTotal > 0)
            {
                notePrefab[i].percentageOfAppearing = (notePrefab[i].percentageOfAppearing / currentOtherTotal) * remainingPercent;
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (notePrefab == null || notePrefab.Length == 0) return;

        float total = 0;
        foreach (var note in notePrefab) total += note.percentageOfAppearing;

        if (total > 0)
        {
            for (int i = 0; i < notePrefab.Length; i++)
            {
                notePrefab[i].percentageOfAppearing = (notePrefab[i].percentageOfAppearing / total) * 100f;
            }
        }
    }
#endif
}