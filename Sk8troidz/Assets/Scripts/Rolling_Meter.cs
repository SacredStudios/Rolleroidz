using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Rolling_Meter : MonoBehaviour
{
    [SerializeField] RectTransform content;  // Reference to the Content GameObject
    private float elementHeight;   // Height of each element
    [SerializeField] float scrollDuration = 1f; // Duration of the scroll
    [SerializeField] GameObject child;
    [SerializeField] int num;

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
        if (number < 0)
        {
            number = 0;
        }
        if (num > 20)
        {
            num = 20;
        }
        elementHeight = child.GetComponent<RectTransform>().rect.height;
        // Target position calculation
        float targetYPosition = 2060 - (number * elementHeight);

        // Start the scrolling coroutine
        StartCoroutine(SmoothScroll(targetYPosition));
    }

    IEnumerator SmoothScroll(float targetYPosition)
    {
        float timeElapsed = 0;
        float startYPosition = content.anchoredPosition.y;

        while (timeElapsed < scrollDuration)
        {
            // Quadratic ease-out interpolation
            float t = timeElapsed / scrollDuration;
            float easedT = 1 - (1 - t) * (1 - t);

            content.anchoredPosition = new Vector2(content.anchoredPosition.x, Mathf.Lerp(startYPosition, targetYPosition, easedT));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetYPosition);
    }
}
