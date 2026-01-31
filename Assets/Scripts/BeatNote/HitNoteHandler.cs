using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitNoteHandler : MonoBehaviour
{
    private Material hitLaneMaterial;
    private Color defaultColor;
    [SerializeField] Material hitMaterial;
    [SerializeField] Material perfectHitMaterial;
    [SerializeField] Material missMaterial;
    [SerializeField] int originalLane;
    [SerializeField] float revertDelay = 0.5f;
    [SerializeField] GameObject vfxPerfectHitObject;
    [SerializeField] GameObject vfxHitObject;
    private Coroutine revertCoroutine;

    private void Start()
    {
        hitLaneMaterial = GetComponent<Renderer>().material;
        defaultColor = hitLaneMaterial.color;
    }

    private void OnEnable()
    {
        NoteEvents.OnNoteHit += NoteHitUpdate;
        NoteEvents.OnNotePerfectHit += NotePerfectHitUpdate;
        NoteEvents.OnNoteMiss += NoteMissUpdate;
    }
    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= NoteHitUpdate;
        NoteEvents.OnNotePerfectHit -= NotePerfectHitUpdate;
        NoteEvents.OnNoteMiss -= NoteMissUpdate;
    }

    private void NoteHitUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = hitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxHitObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
    }

    private void NotePerfectHitUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = perfectHitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxPerfectHitObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
    }

    private void NoteMissUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = missMaterial.color;
        ChangeBackMaterial();
    }

    private void ChangeBackMaterial()
    {
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);
        revertCoroutine = StartCoroutine(RevertColorCoroutine());
    }

    private IEnumerator RevertColorCoroutine()
    {
        yield return new WaitForSeconds(revertDelay);
        hitLaneMaterial.color = defaultColor;
        revertCoroutine = null;
    }
}
