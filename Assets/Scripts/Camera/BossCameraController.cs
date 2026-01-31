using UnityEngine;
using Cinemachine; 
using System.Collections;

public class BossCameraController : MonoBehaviour
{
    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera gameplayCam; 
    public CinemachineVirtualCamera phase2Cam;   
    public CinemachineVirtualCamera phase3Cam;   

    
    public void ShowBossCamera(int phaseNum, float duration)
    {
        if (phaseNum == 2) StartCoroutine(SwitchPriority(phase2Cam, duration));
        if (phaseNum == 3) StartCoroutine(SwitchPriority(phase3Cam, duration));
    }

    private IEnumerator SwitchPriority(CinemachineVirtualCamera targetCam, float duration)
    {
        if (targetCam == null) yield break;

        targetCam.Priority = 20; 

        yield return new WaitForSeconds(duration);

        targetCam.Priority = 0;
    }
}