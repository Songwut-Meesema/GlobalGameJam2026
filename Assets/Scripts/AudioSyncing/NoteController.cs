using UnityEngine;

public class NoteController : MonoBehaviour {
    private float targetTime;
    private Vector3 spawnPos;
    private Vector3 hitPos;
    private float travelDuration;

    // รับ Struct NoteInfo เข้ามาทั้งหมด (แก้ Error CS1503)
    public void Initialize(NoteInfo info, Vector3 start, Vector3 end, float duration) {
        targetTime = info.timeInSeconds;
        spawnPos = start;
        hitPos = end;
        travelDuration = duration;
    }

    void Update() {
        if (Conductor.Instance == null) return;

        float currentTime = Conductor.Instance.songPositionSeconds;
        float t = 1f - ((targetTime - currentTime) / travelDuration);

        if (t >= 0) {
            transform.position = Vector3.Lerp(spawnPos, hitPos, t);
        }

        // Auto Miss
        if (t > 1.15f) {
            RhythmEvents.TriggerNoteMiss();
            Destroy(gameObject);
        }
    }
}