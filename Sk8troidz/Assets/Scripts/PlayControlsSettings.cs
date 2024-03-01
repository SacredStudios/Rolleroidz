using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayControlsSettings : MonoBehaviour
{
    [SerializeField] Slider slider;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("mouse_sensitivity"))
        { 
            ChangeMouseSensitivity();
        }
        else
        {
            Debug.Log(slider.value);
            slider.value = PlayerPrefs.GetFloat("mouse_sensitivity");
        }
    }

    public void ChangeMouseSensitivity()
    {
        
        PlayerPrefs.SetFloat("mouse_sensitivity", slider.value);
        Debug.Log(PlayerPrefs.GetFloat("mouse_sensitivity"));
        if (PlayerPrefs.GetFloat("mouse_sensitivity") == 0)
        {
            PlayerPrefs.SetFloat("mouse_sensitivity", 3.5f);
        }
        PlayerPrefs.Save();
     
    }

}
