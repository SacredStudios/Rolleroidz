using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WordJumper : MonoBehaviour
{
    public Text[] textComponents; // Assign all the Text components in the inspector
    public float jumpHeight = 30f;
    public float durationBetweenJumps = 0.5f;
    public float cycleDelay = 1.0f; // Delay between each full cycle
    private Vector3[] originalPositions;
    public AnimationCurve jumpCurve;

    void Start()
    {
        originalPositions = new Vector3[textComponents.Length];
        for (int i = 0; i < textComponents.Length; i++)
        {
            originalPositions[i] = textComponents[i].transform.position;
        }
        StartCoroutine(AnimateWords());
    }

    IEnumerator AnimateWords()
    {
        while (true)
        {
            for (int i = 0; i < textComponents.Length; i++)
            {
                StartCoroutine(JumpWord(textComponents[i], i));
                yield return new WaitForSeconds(durationBetweenJumps);
            }
            yield return new WaitForSeconds(cycleDelay);
        }
    }

    IEnumerator JumpWord(Text text, int index)
    {
        Vector3 startPos = originalPositions[index];
        Vector3 endPos = new Vector3(startPos.x, startPos.y + jumpHeight, startPos.z);

        float time = 0;
        while (time < durationBetweenJumps)
        {
            float t = time / durationBetweenJumps;
            text.transform.position = Vector3.Lerp(startPos, endPos, jumpCurve.Evaluate(t));
            // Squash and Stretch
            text.transform.localScale = new Vector3(1.0f - jumpCurve.Evaluate(t) * 0.15f, 1.0f + jumpCurve.Evaluate(t) * 0.15f, 1.0f);
            time += Time.deltaTime;
            yield return null;
        }
        // Reset position and scale
        text.transform.position = startPos;
        text.transform.localScale = Vector3.one;
    }
}
