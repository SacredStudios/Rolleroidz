using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stat_Num : MonoBehaviour
{
    [SerializeField] Text amount;
    [SerializeField] Slider slider;
    public void ChangeText()
    {
        amount.text ="" + slider.value;
    }
   
}
