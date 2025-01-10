using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Logo : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image logoImage;

    [Header("Paint Settings")]
    [Tooltip("Total duration (seconds) of the 'painting' reveal.")]
    [SerializeField] private float animationDuration = 2f;

    [Tooltip("Min time (seconds) between fill increments.")]
    [SerializeField] private float minStepDelay = 0.02f;

    [Tooltip("Max time (seconds) between fill increments.")]
    [SerializeField] private float maxStepDelay = 0.1f;

    [Tooltip("Min fill increment each step (0 to 1).")]
    [SerializeField] private float minFillIncrement = 0.01f;

    [Tooltip("Max fill increment each step (0 to 1).")]
    [SerializeField] private float maxFillIncrement = 0.05f;

    private float elapsedTime;

    private void Start()
    {
        if (logoImage != null)
        {
            // Make sure the Image is in "Filled" mode
            // (set in Inspector for best results, but here just in case)
            logoImage.type = Image.Type.Filled;

            // For a left-to-right reveal:
            //   - FillMethod: Horizontal
            //   - FillOrigin: 0 (Left)
            //   - FillAmount: 0
            logoImage.fillMethod = Image.FillMethod.Horizontal;
            logoImage.fillOrigin = 0;
            logoImage.fillAmount = 0f;

            // Start the painting reveal
            StartCoroutine(PaintReveal());
        }
        else
        {
            Debug.LogWarning("Logo Image is not assigned!");
        }
    }

    private IEnumerator PaintReveal()
    {
        elapsedTime = 0f;

        while (elapsedTime < animationDuration && logoImage.fillAmount < 1f)
        {
            // Wait for a random short time between "strokes"
            float waitTime = Random.Range(minStepDelay, maxStepDelay);
            yield return new WaitForSeconds(waitTime);

            // Randomly decide how much of the next "streak" to paint
            float fillInc = Random.Range(minFillIncrement, maxFillIncrement);

            // Apply it
            if (logoImage != null)
            {
                logoImage.fillAmount = Mathf.Clamp01(logoImage.fillAmount + fillInc);
            }

            // Accumulate time
            elapsedTime += waitTime;
        }

        // Ensure the image is fully revealed at the end
        if (logoImage != null)
        {
            logoImage.fillAmount = 1f;
        }
    }
}
