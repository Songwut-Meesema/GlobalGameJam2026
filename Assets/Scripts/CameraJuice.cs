using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJuice : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void OnEnable()
    {
        // แก้ไข: ใช้ Named Method แทน Lambda เพื่อให้ Unsubscribe ได้จริง
        NoteObject.OnNoteHit += HandleHit;
        NoteObject.OnNoteHitPerfect += HandlePerfect;
        NoteObject.OnNoteMiss += HandleMiss;
    }

    private void OnDisable()
    {
        NoteObject.OnNoteHit -= HandleHit;
        NoteObject.OnNoteHitPerfect -= HandlePerfect;
        NoteObject.OnNoteMiss -= HandleMiss;
    }

    private void Start() => originalPos = transform.localPosition;

    // สร้าง Method แยกเพื่อให้ Event เรียกใช้ได้ถูกต้อง
    private void HandleHit(int _) => StartShake(0.08f, 0.1f);
    private void HandlePerfect(int _) => StartShake(0.2f, 0.15f);
    private void HandleMiss(int _) => StartShake(0.4f, 0.2f);

    public void StartShake(float intensity, float duration)
    {
        // เช็คว่า Object ยังมีตัวตนอยู่ไหมก่อนรัน Coroutine
        if (this == null || !gameObject.activeInHierarchy) return;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake(intensity, duration));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // ป้องกัน Error กรณี Object ถูกทำลายระหว่างที่กำลังสั่น
            if (this == null) yield break;

            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (this != null) transform.localPosition = originalPos;
    }
}