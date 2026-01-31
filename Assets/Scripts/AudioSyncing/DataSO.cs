using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NoteInfo {
    public float timeInSeconds; 
    public int laneIndex; 
    public float duration;      
}

[CreateAssetMenu(fileName = "SoData", menuName = "RhythmGame/SongData")]
public class SongData : ScriptableObject {
    public AudioClip musicClip;
    public float bpm;
    public float firstBeatOffset;
    public List<NoteInfo> notes = new List<NoteInfo>(); //data of notes will be hereผ
}