using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PhaseTimedHide {
    public GameObject targetObject;
    public float hideAtSecond; 
}

[System.Serializable]
public struct UniquePhaseData {
    public string phaseName;
    public SongData phaseSongData;      
    public AudioClip phaseMusic;        
    public string animationTrigger;     
    public float showDuration;          
    
    [Header("Phase Audio")]
    public AudioClip transitionSFX; // เสียงที่จะเล่นในช่วงเปลี่ยนเฟส (เช่น เสียงคำราม, เสียงเอฟเฟกต์)

    [Header("Visual Phase Items")]
    public GameObject maskToShow; 
    public List<GameObject> objectsToHideImmediately; 
    public List<PhaseTimedHide> timedHidingActions; 
    public float timeBeforeCameraPan;
}

public class BossPhaseSequencer : MonoBehaviour
{
    // ตัวแปร Static สำหรับตรวจสอบสถานะการเล่น Cinematic (ใช้ Lock Animation ของ Player/Boss)
    public static bool IsCinematicActive { get; private set; }

    [Header("Phase 1 Setup")]
    public List<GameObject> phase1HideImmediately;
    public GameObject phase1Mask;

    [Header("Phase Configuration (Phase 2 & 3)")]
    public List<UniquePhaseData> phaseSettings; 

    [Header("References")]
    public BossCameraController cameraController;
    public NoteSpawner noteSpawner;
    public AudioSource musicSource; // สำหรับเล่นเพลงหลัก
    public AudioSource sfxSource;   // สำหรับเล่นเสียง Transition (สร้าง AudioSource แยกมาอีกอัน)
    public Animator bossAnimator;

    void Start() {
        IsCinematicActive = false;
        if (phase1Mask != null) phase1Mask.SetActive(true);
        foreach (var obj in phase1HideImmediately) if(obj != null) obj.SetActive(false);
    }

    private void OnEnable() => BossPhaseManager.OnPhaseChanged += StartPhaseTransition;
    private void OnDisable() => BossPhaseManager.OnPhaseChanged -= StartPhaseTransition;

    private void StartPhaseTransition(int phaseIndex) {
        // index - 2 เพราะลิสต์เริ่มจากเฟส 2 (index 0 ในลิสต์ = เฟส 2)
        int settingIndex = phaseIndex - 2; 
        if (settingIndex < 0 || settingIndex >= phaseSettings.Count) return;

        StartCoroutine(ExecuteTransition(phaseSettings[settingIndex], phaseIndex));
    }

    private IEnumerator ExecuteTransition(UniquePhaseData settings, int phaseNum) {
        // 1. เริ่มต้นการ Lock และหยุดเกมเพลย์
        IsCinematicActive = true; 
        Debug.Log($"<color=orange>[Sequencer]</color> Starting Phase {phaseNum} - Animation Locked");

        noteSpawner.StopSpawner(); 
        musicSource.Stop();
        
        // 2. เริ่มเล่นเสียง Transition SFX ทันที
        if (sfxSource != null && settings.transitionSFX != null) {
            sfxSource.clip = settings.transitionSFX;
            sfxSource.Play();
        }

        // 3. จัดการ UI และ Mask
        if (settings.maskToShow != null) settings.maskToShow.SetActive(true);
        foreach (var obj in settings.objectsToHideImmediately) if(obj != null) obj.SetActive(false);

        // 4. เล่น Animation เปลี่ยน Phase
        if (bossAnimator != null && !string.IsNullOrEmpty(settings.animationTrigger))
            bossAnimator.SetTrigger(settings.animationTrigger);

        // 5. รอตามเวลาที่ตั้งไว้ก่อนเริ่มแพนกล้อง (ถ้ามี)
        yield return new WaitForSeconds(settings.timeBeforeCameraPan);

        // 6. สั่งกล้องให้ทำงานตามระยะเวลา showDuration
        if (cameraController != null) cameraController.ShowBossCamera(phaseNum, settings.showDuration);

        float timer = 0;
        List<PhaseTimedHide> pendingActions = new List<PhaseTimedHide>(settings.timedHidingActions);

        // 7. ลูปทำงานระหว่างที่ Duration กำลังดำเนินอยู่
        while (timer < settings.showDuration) {
            timer += Time.unscaledDeltaTime;
            
            // ตรวจสอบไอเทมที่ต้องซ่อนตามเวลาที่กำหนดใน List
            for (int i = pendingActions.Count - 1; i >= 0; i--) {
                if (timer >= pendingActions[i].hideAtSecond) {
                    if (pendingActions[i].targetObject != null) pendingActions[i].targetObject.SetActive(false);
                    pendingActions.RemoveAt(i);
                }
            }
            yield return null;
        }

        // 8. หยุดเสียง Transition SFX ทันทีเมื่อ Duration จบลง
        if (sfxSource != null) {
            sfxSource.Stop();
        }

        // 9. รอช่วงพักสั้นๆ 1.5 วิ (เตรียมตัวเริ่มเกม)
        yield return new WaitForSeconds(1.5f);

        // 10. เปลี่ยนข้อมูลเพลงและเริ่ม Spawner ใหม่
        if (settings.phaseSongData != null) {
            noteSpawner.songData = settings.phaseSongData;
            musicSource.clip = settings.phaseMusic;
            noteSpawner.RestartSpawnerWithNewData();
        }

        // 11. ปลดล็อค Cinematic กลับเข้าสู่โหมดเกมเพลย์ปกติ
        IsCinematicActive = false;
        Debug.Log("<color=green>[Sequencer]</color> Transition Finished - Animation Unlocked");
    }
}