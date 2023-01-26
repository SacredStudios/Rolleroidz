using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
    [SerializeField] Text fps_text;
    [SerializeField] bool repeat = true;

    void Start()
    {
        StartCoroutine(FPS());
    }


    IEnumerator FPS()
    {
        while (repeat)
        {
            yield return new WaitForSeconds(0.25f);
            fps_text.text = ((int)(1.0f / Time.deltaTime)).ToString();
        }
       
    }
}
