using UnityEngine;

public class BossVisualController : MonoBehaviour
{
    [SerializeField] private Renderer bossRenderer;
    [SerializeField] private float scaleOffset = 1.1f;
    [SerializeField] private Texture2D texPhase1;
    [SerializeField] private Texture2D texPhase2;
    [SerializeField] private Texture2D texPhase3;

    private void OnEnable() => BossPhaseManager.OnPhaseChanged += ChangeVisual;
    private void OnDisable() => BossPhaseManager.OnPhaseChanged -= ChangeVisual;

    private void ChangeVisual(int phase) {
        if (bossRenderer == null) return;
        
        Texture2D targetTex = texPhase1;
        if (phase == 2) targetTex = texPhase2;
        if (phase == 3) targetTex = texPhase3;

        bossRenderer.material.mainTexture = targetTex;
        
        transform.localScale *= scaleOffset;
    }
}