using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Rolling_Meter : MonoBehaviour
{
    [SerializeField] RectTransform content;  // Reference to the Content GameObject
    private float elementHeight;             // Height of each element
    [SerializeField] float scrollDuration = 1f; // Duration of the scroll
    [SerializeField] GameObject child;
    [SerializeField] int num;

    private Coroutine scrollRoutine;         // ← new handle

    void Start()
    {
        // Calculate the height of the first child (assuming all children are the same size)
        if (content.childCount > 0)
        {
            elementHeight = child.GetComponent<RectTransform>().rect.height;
        }
    }

    public void ScrollToNumber(int number)
    {
        if (number < 0) number = 0;
        if (number > 20) number = 20;        // clamp – was using 'num' by mistake

        // cancel any scroll already running
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        elementHeight = child.GetComponent<RectTransform>().rect.height;

        // Target position calculation
        float targetYPosition = 2060 - (number * elementHeight);

        // Start the scrolling coroutine
        scrollRoutine = StartCoroutine(SmoothScroll(targetYPosition));
    }

    IEnumerator SmoothScroll(float targetYPosition)
    {
        float timeElapsed = 0f;
        float startYPosition = content.anchoredPosition.y;

        while (timeElapsed < scrollDuration)
        {
            // Quadratic ease-out interpolation
            float t = timeElapsed / scrollDuration;
            float easedT = 1f - (1f - t) * (1f - t);

            content.anchoredPosition = new Vector2(
                content.anchoredPosition.x,
                Mathf.Lerp(startYPosition, targetYPosition, easedT));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetYPosition);
        scrollRoutine = null;               // done
    }
}
