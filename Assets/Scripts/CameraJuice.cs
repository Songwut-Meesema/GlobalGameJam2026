using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJuice : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void OnEnable()
    {
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
    private void HandleHit(int _) => StartShake(0.08f, 0.1f);
    private void HandlePerfect(int _) => StartShake(0.2f, 0.15f);
    private void HandleMiss(int _) => StartShake(0.4f, 0.2f);

    public void StartShake(float intensity, float duration)
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake(intensity, duration));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (this == null) yield break;

            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (this != null) transform.localPosition = originalPos;
    }
}