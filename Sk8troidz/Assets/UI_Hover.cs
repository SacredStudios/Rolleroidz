using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform buttonRect; // Button's RectTransform
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f); // Scale on hover
    [SerializeField] private float transitionSpeed = 10f; // Speed of scaling transition

    private Vector3 originalScale; // Original scale of the button

    private void Start()
    {
        // Get the RectTransform if not already assigned
        if (buttonRect == null)
        {
            buttonRect = GetComponent<RectTransform>();
        }

        // Store the original scale
        originalScale = buttonRect.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up when mouse enters
        StopAllCoroutines(); // Stop any ongoing scaling animation
        StartCoroutine(ScaleButton(hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale back to normal when mouse exits
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale));
    }

    private System.Collections.IEnumerator ScaleButton(Vector3 targetScale)
    {
        while (Vector3.Distance(buttonRect.localScale, targetScale) > 0.01f)
        {
            buttonRect.localScale = Vector3.Lerp(buttonRect.localScale, targetScale, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        buttonRect.localScale = targetScale; // Ensure the exact target scale is set
    }
}

