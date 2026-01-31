using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBomb : Notebase
{
    protected override void CheckHit()
    {
        // Debug.Log("Enter Bomb Hit Logic");
        float absOffset = Mathf.Abs(Conductor.Instance.songPositionSeconds - hitTime);
        
        if (absOffset < 0.20f)
        {
            // Debug.Log("Bomb Hit Detected");
            isHandled = true;
            NoteEvents.TriggerBombHit(laneIndex);
            Destroy(gameObject);
        }
    }

    protected override void OnMiss()
    {
        isHandled = true;
        Destroy(gameObject);
    }
}
