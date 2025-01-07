using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WordJumper : MonoBehaviour
{
    public RectTransform[] uiElements; // Assign all the UI elements in the inspector
    public float jumpHeight = 30f;
    public float durationBetweenJumps = 0.5f;
    public float cycleDelay = 1.0f; // Delay between each full cycle
    public AnimationCurve jumpCurve;

    // Instead of storing Vector3 for world positions, store Vector2 for anchored positions
    private Vector2[] originalAnchoredPositions;

    void Start()
    {
        originalAnchoredPositions = new Vector2[uiElements.Length];
        for (int i = 0; i < uiElements.Length; i++)
        {
            // Store each UI element's anchored position
            originalAnchoredPositions[i] = uiElements[i].anchoredPosition;
        }

        // Start animating
        StartCoroutine(AnimateWords());
    }

    IEnumerator AnimateWords()
    {
        while (true)
        {
            for (int i = 0; i < uiElements.Length; i++)
            {
                // In case the array was cleared or changed
                if (uiElements.Length == 0)
                    yield break;

                // Animate the element
                StartCoroutine(JumpElement(uiElements[i], i));
                yield return new WaitForSeconds(durationBetweenJumps);
            }
            yield return new WaitForSeconds(cycleDelay);
        }
    }

    IEnumerator JumpElement(RectTransform element, int index)
    {
        Vector2 startPos = originalAnchoredPositions[index];
        Vector2 endPos = new Vector2(startPos.x, startPos.y + jumpHeight);

        float time = 0;
        while (time < durationBetweenJumps)
        {
            // In case the array was cleared or changed mid-animation
            if (uiElements.Length == 0)
                yield break;

            float t = time / durationBetweenJumps;
            // Use the AnimationCurve for smooth motion
            float curveValue = jumpCurve.Evaluate(t);

            // Animate the anchoredPosition instead of transform.position
            element.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);

            // Squash and stretch
            element.localScale = new Vector3(
                1.0f - curveValue * 0.15f,
                1.0f + curveValue * 0.15f,
                1.0f
            );

            time += Time.deltaTime;
            yield return null;
        }

        // Reset anchoredPosition and scale
        element.anchoredPosition = startPos;
        element.localScale = Vector3.one;
    }
}
