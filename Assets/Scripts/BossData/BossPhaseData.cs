using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BossPhaseSettings
 {
    public string phaseName;
    public SongData phaseSongData;      
    public AudioClip phaseMusic;        
    public string animationTrigger;    
    public float showDuration;         
}
