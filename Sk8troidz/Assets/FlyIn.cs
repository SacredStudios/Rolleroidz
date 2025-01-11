using UnityEngine;

public class FlyIn : MonoBehaviour
{
    public Vector2 startOffset = new Vector2(0, -100); // Starting offset from the final position
    public float duration = 1.0f; // Duration of the fly-in effect in seconds
    public AnimationCurve motionCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1.1f), // Overshoot to 110% of the way at half time
        new Keyframe(1, 1)        // Settle back at 100% at the end
    );

    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private float timer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("FlyIn: No RectTransform found on the object.");
            return;
        }

        // Calculate target position based on the current anchored position
        targetPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = targetPosition + startOffset;
        timer = 0;
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp(timer / duration, 0, 1);
            float curveValue = motionCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(targetPosition + startOffset, targetPosition, curveValue);
        }
    }
}
