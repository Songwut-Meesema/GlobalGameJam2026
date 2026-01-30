using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{

    [SerializeField] private float beatTeampo;
    private bool hasStarted;

    private void Start()
    {
        beatTeampo = beatTeampo / 60f;
    }

    private void Update()
    {
        if(!hasStarted)
        {
            hasStarted = true;
        }
        else
        {
            transform.position += new Vector3( beatTeampo * Time.deltaTime,0, 0);
        }
    }
}
