using UnityEngine;

public class NoteFactory : MonoBehaviour {
    [SerializeField] private SongData songData; // อ้างอิง SO โดยตรง
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] hitPoints;
    [SerializeField] private float noteTravelTime = 2.0f; 
    
    private int nextNoteIndex = 0;

    void Update() {
        if (Conductor.Instance == null || songData == null) return;

        if (nextNoteIndex < songData.notes.Count) {
            // เช็คเวลาเกิด: ถ้าถึงเวลาที่ต้องกด - เวลาเดินทาง
            if (Conductor.Instance.songPositionSeconds >= songData.notes[nextNoteIndex].timeInSeconds - noteTravelTime) {
                SpawnTile(songData.notes[nextNoteIndex]);
                nextNoteIndex++;
            }
        }
    }

    void SpawnTile(NoteInfo info) {
        GameObject tile = Instantiate(notePrefab);
        // ส่ง info (Struct) ไปยัง Initialize (แก้ Error ตรงนี้)
        tile.GetComponent<NoteController>().Initialize(info, spawnPoints[info.laneIndex].position, hitPoints[info.laneIndex].position, noteTravelTime);
    }
}