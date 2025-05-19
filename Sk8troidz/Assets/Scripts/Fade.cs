using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class Fade : MonoBehaviour
{
    [Header("Fade settings")]
    [Tooltip("Peak alpha that will be reached in the middle of the fade.")]
    [Range(0f, 1f)]
    [SerializeField] private float targetAlpha = 1f;

    [Tooltip("Seconds to reach peak alpha (it will fade out over the same time).")]
    [SerializeField] private float fadeDuration = 0.35f;

    [Tooltip("Optional: destroy this GameObject after the fade completes.")]
    [SerializeField] private bool destroyOnFinish = false;

    /* ───────────── internal ───────────── */
    private Graphic graphic;
    private float baseAlpha;
    private Coroutine fadeRoutine;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        baseAlpha = graphic.color.a;         // remember original alpha
    }

    /// <summary>Call this to trigger one fade-in-out pulse.</summary>
    public void Start_Fade()
    {
        if (fadeRoutine == null)
        {
            fadeRoutine = StartCoroutine(FadePulse());
        }
    }

    IEnumerator FadePulse()
    {
        // phase 1: fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            float k = t / fadeDuration;                  // 0 → 1
            SetAlpha(Mathf.Lerp(baseAlpha, targetAlpha, k));
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // phase 2: fade out
        fadeRoutine = null;
        t = 0f;
        while (t < fadeDuration)
        {
            float k = t / fadeDuration;                  // 0 → 1
            SetAlpha(Mathf.Lerp(targetAlpha, baseAlpha, k));
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(baseAlpha);                             // safety
        

        if (destroyOnFinish)
            Destroy(gameObject);
    }

    void SetAlpha(float a)
    {
        Color c = graphic.color;
        c.a = a;
        graphic.color = c;
    }
}
