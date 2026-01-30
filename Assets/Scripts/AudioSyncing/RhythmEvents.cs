using System;
using UnityEngine;

public static class RhythmEvents {
    public static event Action OnNoteHit;
    public static event Action OnNoteMiss;

    public static void TriggerNoteHit() => OnNoteHit?.Invoke();
    public static void TriggerNoteMiss() => OnNoteMiss?.Invoke();
}