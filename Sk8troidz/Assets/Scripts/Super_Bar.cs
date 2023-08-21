using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Super_Bar : MonoBehaviour
{
    static Slider slider;
    public static void ChangeAmount(float new_amount)
    {
        slider.value += new_amount;
    }
    private void Start()
    {
        slider = this.gameObject.GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
