using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class Glow : MonoBehaviour
{
    [Header("Glow look")]
    [ColorUsage(false, true)]
    [SerializeField] private Color glowColor = new Color(1f, 0.95f, 0.2f, 1f);
    [SerializeField] private float glowThickness = 6f;   // pixel distance

    [Header("Glow timing")]
    [Tooltip("Seconds to reach full intensity (it will fade back over the same time).")]
    [SerializeField] private float glowDuration = 0.15f;

    /* ---------------------------------------------------------- */

    Outline outline;         // the halo component
    Coroutine glowRoutine;

    void Awake()
    {
        // add (or fetch) an Outline component once
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectDistance = new Vector2(glowThickness, glowThickness);
        outline.useGraphicAlpha = false;     // let us drive alpha ourselves
        outline.effectColor = glowColor;     // zero alpha initially
    }

    /// <summary>Call this from button, animation, or code to flash the glow.</summary>
    public void Start_Glow()
    {
        if (glowRoutine != null) return;
        glowRoutine = StartCoroutine(PulseGlow());
    }

    IEnumerator PulseGlow()
    {
        // half-cycle brighten
        float t = 0f;
        while (t < glowDuration)
        {
            float k = t / glowDuration;                    // 0 → 1
            SetGlowAlpha(k);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // half-cycle fade
        glowRoutine = null;
        t = 0f;
        while (t < glowDuration)
        {
            float k = 1f - (t / glowDuration);             // 1 → 0
            SetGlowAlpha(k);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        SetGlowAlpha(0f);
    }

    void SetGlowAlpha(float a)
    {
        Color c = glowColor;
        c.a = a;
        outline.effectColor = c;
    }
}
