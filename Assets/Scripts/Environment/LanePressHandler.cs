using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanePressHandler : MonoBehaviour
{
    [SerializeField] private Color laneColor;
    [SerializeField] private float emissionStrength = 1.0f;
    [SerializeField] private int laneIndex; 

    private Color originalColor;

    private Renderer rend;
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    private void OnEnable()
    {
        ButtonController.OnPlayerHit += ChangeLaneColor;
    }
    private void OnDisable()
    {
        ButtonController.OnPlayerHit -= ChangeLaneColor;
    }
    private void ChangeLaneColor(int lane)
    {
        // Debug.Log("ChangeLaneColor: " + lane);
        if (lane != laneIndex) return;
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", laneColor * emissionStrength);
        StartCoroutine(ResetEmissionAfterDelay(0.2f));
    }

    private IEnumerator ResetEmissionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rend.material.color = originalColor;
        rend.material.DisableKeyword("_EMISSION");
        
    }
}
