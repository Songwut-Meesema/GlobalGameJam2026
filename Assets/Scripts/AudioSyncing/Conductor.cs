using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    public SongData songData;
    public AudioSource musicSource;

    public float songPositionSeconds;
    public float dspSongTime;

    private double pauseStartTime;
    private double totalPausedTime;
    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (songData == null) return;
        musicSource.clip = songData.musicClip;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    public void PauseSong()
    {
        isPaused = true;
        pauseStartTime = AudioSettings.dspTime;
        musicSource.Pause();
    }

    public void ResumeSong()
    {
        isPaused = false;
        // Calculate how long we were paused and add it to the offset
        totalPausedTime += (AudioSettings.dspTime - pauseStartTime);
        musicSource.UnPause();
    }
    void Update()
    {
        if (musicSource.isPlaying)
        {
            // เวลาปัจจุบัน = (เวลาจริง - เวลาเริ่ม) - ค่าชดเชย
            songPositionSeconds = (float)(AudioSettings.dspTime - dspSongTime - totalPausedTime) - songData.firstBeatOffset;
        }
    }
}