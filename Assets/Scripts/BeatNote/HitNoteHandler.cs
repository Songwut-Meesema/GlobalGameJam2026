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
    [SerializeField] GameObject vfxHitHealingObject;
    [SerializeField] GameObject vfxHitBombObject;
    [SerializeField] SoundData hitsound;
    [SerializeField] SoundData missSound;

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
        NoteEvents.OnHealingNoteHit += HealNoteHitUpdate;
        NoteEvents.OnBombHit += BombNoteHitUpdate;
    }
    private void OnDisable()
    {
        NoteEvents.OnNoteHit -= NoteHitUpdate;
        NoteEvents.OnNotePerfectHit -= NotePerfectHitUpdate;
        NoteEvents.OnNoteMiss -= NoteMissUpdate;
        NoteEvents.OnHealingNoteHit -= HealNoteHitUpdate;
        NoteEvents.OnBombHit -= BombNoteHitUpdate;
    }

    private void NoteHitUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = hitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxHitObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
        AudioManager.Instance.PlaySound(hitsound);
    }

    private void NotePerfectHitUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = perfectHitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxPerfectHitObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
        AudioManager.Instance.PlaySound(hitsound);

    }

    private void HealNoteHitUpdate(int lane, float amount)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = hitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxHitHealingObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
        AudioManager.Instance.PlaySound(hitsound);

    }

    private void BombNoteHitUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = hitMaterial.color;
        GameObject vfxInstance = Instantiate(vfxHitBombObject, transform.position, Quaternion.identity);
        Destroy(vfxInstance, 1f);
        ChangeBackMaterial();
        AudioManager.Instance.PlaySound(missSound);

    }

    private void NoteMissUpdate(int lane)
    {
        if (originalLane != lane)
            return;
        hitLaneMaterial.color = missMaterial.color;
        ChangeBackMaterial();
        AudioManager.Instance.PlaySound(missSound);

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
