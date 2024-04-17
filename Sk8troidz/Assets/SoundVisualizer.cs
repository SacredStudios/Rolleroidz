using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVisualizer : MonoBehaviour
{
    [SerializeField] AudioSource audio;
    public float[] samples = new float[512];
    public GameObject[] bars = new GameObject[8];
    void Start()
    {
        StartCoroutine(TransformRect());
    }
    IEnumerator TransformRect()
    {
        while (true)
        {
            audio.GetSpectrumData(samples, 0, FFTWindow.Blackman);
            for (int i = 0; i < bars.Length; i++)
            {
                RectTransform rectTransform = bars[i].GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Avg(i));
            }

            yield return new WaitForSeconds(0.1f);
        }
        
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    float Avg(int i)
    {

        float result = 0;
        
        for (int j = (512 / bars.Length) * i; j< (512.0 / bars.Length) * (i+1); j++)
        {
            result += samples[j];
            
        }
        

        return 50f*(-1*Mathf.Log(result,2));
    }
}
