using UnityEngine;

public class NoteFactory : MonoBehaviour {
    [SerializeField] private SongData songData; 
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] hitPoints;
    [SerializeField] private float noteTravelTime = 2.0f; 
    
    private int nextNoteIndex = 0;

    void Update() {
        if (Conductor.Instance == null || songData == null) return;

        while (nextNoteIndex < songData.notes.Count && 
               Conductor.Instance.songPositionSeconds >= songData.notes[nextNoteIndex].timeInSeconds - noteTravelTime) 
        {
            SpawnTile(songData.notes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    void SpawnTile(NoteInfo info) {
        if (info.laneIndex < 0 || info.laneIndex >= spawnPoints.Length) return;

        GameObject tile = Instantiate(notePrefab);
        tile.GetComponent<NoteController>().Initialize(
            info, 
            spawnPoints[info.laneIndex].position, 
            hitPoints[info.laneIndex].position, 
            noteTravelTime
        );
    }
}