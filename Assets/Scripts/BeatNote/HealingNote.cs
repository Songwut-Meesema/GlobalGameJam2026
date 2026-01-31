using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingNote : Notebase
{
    [SerializeField] private float healAmount = 15;
    protected override void CheckHit()
    {
        float absOffset = Mathf.Abs(Conductor.Instance.songPositionSeconds - hitTime);

        if (absOffset < 0.25f)
        {
            isHandled = true;
            NoteEvents.TriggerHeal(laneIndex, healAmount); 
            Destroy(gameObject);
        }
    }
}
