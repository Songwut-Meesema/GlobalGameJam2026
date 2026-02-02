using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndManagerScene : MonoBehaviour
{
    [Header("Mask Sequence")]
    public List<Renderer> maskRenderers; 
    public float fadeSpeed = 1.0f;
    public float floatDist = 0.3f;

    [Header("Dialogue UI")]
    public TextMeshProUGUI dialogueText;
    public List<string> lines; 
    public float typeSpeed = 0.05f;
    public GameObject continuePrompt;

    private int lineIndex = 0;
    private bool isTyping = false;
    private bool introSequenceDone = false;

    private Dictionary<Renderer, List<Color>> maskBaseColors = new Dictionary<Renderer, List<Color>>();

    void Awake()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator Start()
    {
        // 1. รอให้ระบบฉากพร้อม
        yield return null;

        // 2. ตั้งค่าเริ่มต้น (ซ่อนหน้ากากไว้ก่อน)
        foreach (var r in maskRenderers)
        {
            List<Color> colors = new List<Color>();
            foreach (var mat in r.materials)
            {
                Color c = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : 
                         (mat.HasProperty("_Color") ? mat.color : Color.white);
                colors.Add(new Color(c.r, c.g, c.b, 1f));
            }
            maskBaseColors[r] = colors;

            SetAlpha(r, 0f);
            r.transform.position -= Vector3.up * floatDist;
        }

        dialogueText.text = "";
        if (continuePrompt != null) continuePrompt.SetActive(false);

        // --- เพิ่มตรงนี้: รอ 1 วินาทีก่อนเริ่มขยับหน้ากากอันแรก ---
        Debug.Log("Intro waiting for 1 second...");
        yield return new WaitForSecondsRealtime(1.0f); 

        // 3. เริ่มเล่นลำดับหน้ากาก
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        foreach (var r in maskRenderers)
        {
            yield return StartCoroutine(FadeIn(r));
            yield return new WaitForSecondsRealtime(0.5f); 
        }
        
        introSequenceDone = true;
        StartCoroutine(TypeText());
    }

    // --- ส่วนที่เหลือของสคริปต์เหมือนเดิม ---

    IEnumerator FadeIn(Renderer r)
    {
        float alpha = 0;
        Vector3 startPos = r.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDist;

        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed; 
            SetAlpha(r, alpha);
            r.transform.position = Vector3.Lerp(startPos, endPos, alpha);
            yield return null;
        }
        SetAlpha(r, 1f);
    }

    void SetAlpha(Renderer r, float a)
    {
        if (!maskBaseColors.ContainsKey(r)) return;
        for (int i = 0; i < r.materials.Length; i++)
        {
            Material m = r.materials[i];
            Color baseColor = maskBaseColors[r][i];
            Color targetColor = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", targetColor);
            else if (m.HasProperty("_Color")) m.color = targetColor;
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        if (continuePrompt != null) continuePrompt.SetActive(false);
        dialogueText.text = "";
        foreach (char letter in lines[lineIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        isTyping = false;
        if (continuePrompt != null) continuePrompt.SetActive(true);
    }

    void Update()
    {
        if (!introSequenceDone) return;
        if ((Input.anyKeyDown || Input.GetMouseButtonDown(0)) && !isTyping)
        {
            lineIndex++;
            if (lineIndex < lines.Count) StartCoroutine(TypeText());
            else
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}