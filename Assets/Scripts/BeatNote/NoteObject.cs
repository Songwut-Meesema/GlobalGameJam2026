using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private bool canBePressed;
    public NoteType requiredType; 
    public static event Action OnNoteHit;
    public static event Action OnNoteMiss;
    void OnEnable()
    {
        ButtonController.OnPlayerHit += OnNoteEnterPlayer;
    }

    private void OnNoteEnterPlayer(NoteType type)
    {
        if(canBePressed && requiredType.Equals(type))
        {
            OnNoteHit?.Invoke();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Activator"))
        {
            canBePressed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("Activator"))
        {
            canBePressed = false;
        }
    }
}
