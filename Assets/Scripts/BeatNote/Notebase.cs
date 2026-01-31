using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Notebase : MonoBehaviour
{
    [Header("Core Settings")]
    public float hitTime;
    public int laneIndex;
    protected Transform spawnPoint;
    protected Transform hitPoint;
    protected float travelTime;
    protected bool isHandled = false;

    public virtual void Initialize(float targetTime, int lane, Transform spawn, Transform hit, float travel)
    {
        hitTime = targetTime;
        laneIndex = lane;
        spawnPoint = spawn;
        hitPoint = hit;
        travelTime = travel;
    }

    protected virtual void OnEnable() => ButtonController.OnPlayerHit += HandleInput;
    protected virtual void OnDisable() => ButtonController.OnPlayerHit -= HandleInput;

    protected void HandleInput(int inputLane)
    {
        if (isHandled || inputLane != laneIndex) return;
        CheckHit();
    }

    protected abstract void CheckHit(); 

    protected virtual void Update()
    {
        if (isHandled) return;

        float songTime = Conductor.Instance.songPositionSeconds;
        float progress = 1f - ((hitTime - songTime) / travelTime);
        transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, Mathf.Clamp01(progress));

        if (songTime - hitTime > 0.15f)
        {
            OnMiss();
        }
    }

    protected virtual void OnMiss()
    {
        isHandled = true;
        gameObject.SetActive(false);
        NoteEvents.TriggerMiss(laneIndex);
        Destroy(gameObject, 0.2f);
    }
}

public static class NoteEvents
{
    // ใช้ Action ตัวเดียวที่ครอบคลุมทุกอย่าง หรือแยกตามประเภทก็ได้
    public static Action<int> OnNoteHit;
    public static Action<int> OnNotePerfectHit;
    public static Action<int> OnNoteMiss;
    
    // สำหรับ Note พิเศษ
    public static Action<int> OnBombHit; 

    // Helper methods เพื่อให้เรียกง่ายขึ้น
    public static void TriggerNormalHit(int lane) => OnNoteHit?.Invoke(lane);
    public static void TriggerPerfectHit(int lane) => OnNotePerfectHit?.Invoke(lane);
    public static void TriggerMiss(int lane) => OnNoteMiss?.Invoke(lane);
    public static void TriggerBombHit(int lane) => OnBombHit?.Invoke(lane);
}