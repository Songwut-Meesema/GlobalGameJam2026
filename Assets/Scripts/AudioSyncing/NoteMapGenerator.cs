using System.Collections.Generic;
using UnityEngine;

public class NoteMapGenerator : MonoBehaviour
{
    [Header("Target Data")]
    public SongData targetSoData;
    public TextAsset midiJsonFile;

    [Header("Balance & Game Feel")]
    [Tooltip("Minimum gap between notes in the same lane (Seconds). Adjust this to prevent overlapping.")]
    public float minInterval = 0.15f;

    // ContextMenu 
    [ContextMenu("Generate Pattern Now")]
    public void GeneratePattern()
    {
        if (targetSoData == null || midiJsonFile == null)
        {
            Debug.LogError("[Architect] Error: Missing SoData or JSON file in the Inspector!");
            return;
        }

        // Deserialize JSON to C# Objects
        MidiJsonData data = JsonUtility.FromJson<MidiJsonData>(midiJsonFile.text);

        // Update SongData
        targetSoData.bpm = data.header.bpm;
        targetSoData.notes.Clear();

        // Tracker for filtering (Lane ID -> Last Time)
        Dictionary<int, float> lastTimeInLane = new Dictionary<int, float>() {
            {0, -1f}, {1, -1f}, {2, -1f}, {3, -1f}
        };

        foreach (var track in data.tracks)
        {
            foreach (var n in track.notes)
            {
                int lane = -1;

                // Map MIDI Pitch to Lane Index (0-3)
                if (n.midi == 31) lane = 0;
                else if (n.midi == 43) lane = 1;
                else if (n.midi == 50) lane = 2;
                else if (n.midi == 55) lane = 3;

                if (lane != -1)
                {
                    // BALANCE LOGIC: Prevent notes from being too close together
                    if (n.time >= lastTimeInLane[lane] + minInterval)
                    {
                        NoteInfo newNote = new NoteInfo();
                        newNote.timeInSeconds = n.time;
                        newNote.laneIndex = lane;

                        targetSoData.notes.Add(newNote);
                        lastTimeInLane[lane] = n.time; // Update last timestamp
                    }
                }
            }
        }

        // Sort by time to ensure Factory plays them in order
        targetSoData.notes.Sort((a, b) => a.timeInSeconds.CompareTo(b.timeInSeconds));

        // Save changes to the Asset file
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetSoData);

        // บังคับเขียนข้อมูลจาก RAM ลงสู่ไฟล์ .asset บน Hard Drive
        UnityEditor.AssetDatabase.SaveAssets();

        // รีเฟรชฐานข้อมูล Assets ของ Unity
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log("<color=green>[Success]</color> ข้อมูลถูกบันทึกลงไฟล์ Hard Drive เรียบร้อยแล้ว เปลี่ยน Scene ได้ไม่พังแน่นอน");
#endif

        Debug.Log($"<color=green>[Architect]</color> Successfully generated {targetSoData.notes.Count} notes into {targetSoData.name}");
    }
}

// --- Helper Classes for JSON Parsing (DO NOT REMOVE) ---
[System.Serializable]
public class MidiJsonData
{
    public MidiHeader header;
    public List<MidiTrack> tracks;
}

[System.Serializable]
public class MidiHeader
{
    public float bpm;
}

[System.Serializable]
public class MidiTrack
{
    public List<MidiNote> notes;
}

[System.Serializable]
public class MidiNote
{
    public float time;
    public int midi;
}