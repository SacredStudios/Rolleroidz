using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro.Examples;

public class Text_Reveal : MonoBehaviour
{
    [SerializeField] private Text textComponent; // Assign this in the Inspector
    [SerializeField] private float revealDelay = 0.1f; // Time delay between each letter

    private string fullText;
    private Coroutine revealCoroutine;

    void OnEnable()
    {
        

        // Prevent multiple coroutines from running simultaneously
        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
        } else
        {
            fullText = textComponent.text;
            textComponent.text = "";
            
        }
        revealCoroutine = StartCoroutine(RevealText());



    }

    private IEnumerator RevealText()
    {
        Debug.Log("This is a test for coroutine");
        for (int i = 0; i <= fullText.Length; i++)
        {
            Debug.Log("This is the " + i +"th test for coroutine");
            textComponent.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(revealDelay);
        }
    }
}
