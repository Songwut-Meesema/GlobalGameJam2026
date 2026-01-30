using System.Collections.Generic;
using UnityEngine;

public class NoteMapGenerator : MonoBehaviour
{
    [Header("Target Data")]
    public SongData targetSoData; 
    public TextAsset midiJsonFile; 

    [ContextMenu("Generate Pattern Now")] // คำสั่งนี้จะโผล่มาให้กด!
    public void GeneratePattern()
    {
        if (targetSoData == null || midiJsonFile == null)
        {
            Debug.LogError("Error: ลืมลาก SoData หรือไฟล์ JSON มาใส่ใน Inspector หรือเปล่าครับ?");
            return;
        }

        //JSON READER
        MidiJsonData data = JsonUtility.FromJson<MidiJsonData>(midiJsonFile.text);
        
        //overwrite SO Data
        targetSoData.bpm = data.header.bpm;
        targetSoData.notes.Clear();

        foreach (var track in data.tracks) {
            foreach (var n in track.notes) {
                NoteInfo newNote = new NoteInfo();
                newNote.timeInSeconds = n.time;
                
                // Map MIDI Note to Lane
                if (n.midi == 31) newNote.laneIndex = 0;
                else if (n.midi == 43) newNote.laneIndex = 1;
                else if (n.midi == 50) newNote.laneIndex = 2;
                else if (n.midi == 55) newNote.laneIndex = 3;
                else continue;

                targetSoData.notes.Add(newNote);
            }
        }

        targetSoData.notes.Sort((a, b) => a.timeInSeconds.CompareTo(b.timeInSeconds));
        
        // Save changes to SO asset 
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetSoData);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"<color=green>[Architect]</color> เจนโน้ตลง {targetSoData.name} สำเร็จ! ทั้งหมด {targetSoData.notes.Count} ตัว");
    }
}

// pust outside classes for JSON parsing
[System.Serializable] public class MidiJsonData { public MidiHeader header; public List<MidiTrack> tracks; }
[System.Serializable] public class MidiHeader { public float bpm; }
[System.Serializable] public class MidiTrack { public List<MidiNote> notes; }
[System.Serializable] public class MidiNote { public float time; public int midi; }