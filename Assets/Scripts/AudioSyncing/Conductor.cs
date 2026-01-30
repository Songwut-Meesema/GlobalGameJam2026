using UnityEngine;

public class Conductor : MonoBehaviour {
    public static Conductor Instance;
    public SongData songData;
    public AudioSource musicSource;

    public float songPositionSeconds;
    public float dspSongTime;

    private void Awake() => Instance = this;

    void Start() {
        if (songData == null) return;
        musicSource.clip = songData.musicClip;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    void Update() {
        if (musicSource.isPlaying) {
            // หัวใจของการ Sync: เวลาปัจจุบัน = (เวลาจริง - เวลาเริ่ม) - ค่าชดเชย
            songPositionSeconds = (float)(AudioSettings.dspTime - dspSongTime) - songData.firstBeatOffset;
        }
    }
}