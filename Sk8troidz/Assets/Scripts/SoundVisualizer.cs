using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audio;      // your music / mic source
    [SerializeField] private GameObject[] bars = new GameObject[8];

    [Header("Visual settings")]
    [Tooltip("Vertical scale factor for the bars (both modes).")]
    [SerializeField] private float yScale = 300f;
    [Tooltip("Smallest bar height in pixels.")]
    [SerializeField] private float minHeight = 2f;

    private readonly float[] samples = new float[512];   // FFT or output buffer

    /* ------------------------------------------------------------ */
    private void Start() => StartCoroutine(DriveBars());

    /* ------------------------------------------------------------ */
    private IEnumerator DriveBars()
    {
        var wait = new WaitForSeconds(0.02f); // ~50 fps

        while (true)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            /* -------------- WebGL fallback: RMS meter -------------- */
            audio.GetOutputData(samples, 0);          // this works on WebGL

            int slice = samples.Length / bars.Length;
            for (int i = 0; i < bars.Length; i++)
            {
                float sum = 0f;
                int start = i * slice;
                int end   = start + slice;

                for (int j = start; j < end; j++)
                    sum += samples[j] * samples[j];   // power

                float rms = Mathf.Sqrt(sum / slice);  // 0-1
                SetBarHeight(i, rms * yScale);
            }
#else
            /* -------- Editor / PC / mobile: original FFT path ------ */
            audio.GetSpectrumData(samples, 0, FFTWindow.Blackman);

            int slice = samples.Length / bars.Length;
            for (int i = 0; i < bars.Length; i++)
            {
                float sum = 0f;
                int start = i * slice;
                int end = start + slice;

                for (int j = start; j < end; j++)
                    sum += samples[j];

                // your original log-scaled formula
                float h = 50f * (-Mathf.Log(sum + 1e-8f, 2));
                SetBarHeight(i, h);
            }
#endif
            yield return wait;
        }
    }

    /* ------------------------------------------------------------ */
    private void SetBarHeight(int index, float height)
    {
        if (index < 0 || index >= bars.Length || bars[index] == null) return;

        var rt = bars[index].GetComponent<RectTransform>();
        if (!rt) return;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x,
                                   Mathf.Max(minHeight, height));
    }
}
