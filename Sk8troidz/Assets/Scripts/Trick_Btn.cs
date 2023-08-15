using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trick_Btn : MonoBehaviour//makes sure weapon is not fired whenever trick button is clicked
{
    [SerializeField] GameObject Weapon_Handler;
    
    
    void Update()
    {
        if (PlayerMovement.trick_mode_activated == false)
        {
            Destroy(this.gameObject);
        }
    }
    
}