using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Cooldown : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Slider slider_other;
    [SerializeField] float increment;
    [SerializeField] bool game_ongoing;
    [SerializeField] float max_value;
    [SerializeField] Player_Health ph;
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
                ph.Add_Health(increment);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}