using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class Text_Manager : MonoBehaviour
{
    public GameObject current_text;
    public void SetText(GameObject text) 
    {
        Debug.Log("setting text");
        if (current_text != null)
        {
            current_text.SetActive(false);
        }
        text.SetActive(true);
        current_text = text;
    }
   
}
