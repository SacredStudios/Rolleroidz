using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trick_Btn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler //makes sure weapon is not fired whenever trick button is clicked
{
    [SerializeField] GameObject Weapon_Handler;
    private Weapon_Handler wh;
    void Start()
    {
        wh = Weapon_Handler.GetComponent<Weapon_Handler>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        wh.isOverTrickBtn= true;
        Debug.Log("on");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        wh.isOverTrickBtn = false;
        Debug.Log("off");

    }
}