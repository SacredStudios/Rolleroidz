using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Cooldown : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] float increment;
    [SerializeField] bool game_ongoing;
    [SerializeField] float max_value;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cooldown());
    }

    // Update is called once per frame
    IEnumerator Cooldown()
    {
        while (game_ongoing)
        {
            if (slider.value > 0)
            {
                slider.value += increment;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}