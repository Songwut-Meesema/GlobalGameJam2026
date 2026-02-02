using UnityEngine;
using Cinemachine; 
using System.Collections;

public class BossCameraController : MonoBehaviour
{
    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera gameplayCam; 
    public CinemachineVirtualCamera phase2Cam;   
    public CinemachineVirtualCamera phase3Cam;
    public CinemachineVirtualCamera deathCam; // กล้องสำหรับฉากบอสตาย (Phase 4)

    [Header("Settings")]
    public int activePriority = 20;
    public int inactivePriority = 0;

    /// <summary>
    /// เรียกใช้งานกล้องตามเลข Phase (2, 3 = Phase Change, 4 = Death)
    /// </summary>
    public void ShowBossCamera(int phaseNum, float duration)
    {
        CinemachineVirtualCamera targetCam = null;

        // เลือกกล้องตามรหัสที่ส่งมา
        switch (phaseNum)
        {
            case 2: targetCam = phase2Cam; break;
            case 3: targetCam = phase3Cam; break;
            case 4: targetCam = deathCam;  break;
        }

        if (targetCam != null)
        {
            StartCoroutine(SwitchPriority(targetCam, duration));
        }
        else
        {
            Debug.LogWarning($"[BossCameraController] No camera assigned for phase: {phaseNum}");
        }
    }

    private IEnumerator SwitchPriority(CinemachineVirtualCamera targetCam, float duration)
    {
        // ดัน Priority ขึ้นเพื่อให้ Cinemachine ตัดไปที่กล้องนี้
        targetCam.Priority = activePriority; 

        yield return new WaitForSeconds(duration);

        // คืนค่า Priority เพื่อให้กลับไปใช้กล้อง Gameplay หลัก
        targetCam.Priority = inactivePriority;
    }
}