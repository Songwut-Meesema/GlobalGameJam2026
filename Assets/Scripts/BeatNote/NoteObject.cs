using System;
using UnityEngine;

public class NoteObject : Notebase
{
    protected override void CheckHit()
    {
        float absOffset = Mathf.Abs(Conductor.Instance.songPositionSeconds - hitTime);

        if (absOffset < 0.25f)
        {
            isHandled = true;
            if (absOffset < 0.12f)
                NoteEvents.TriggerPerfectHit(laneIndex); // ส่งผ่าน Hub
            else
                NoteEvents.TriggerNormalHit(laneIndex); // ส่งผ่าน Hub

            Destroy(gameObject);
        }
    }
}
