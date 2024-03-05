using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] GameObject panel;
 public void Show()
    {
        panel.SetActive(true);
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}
