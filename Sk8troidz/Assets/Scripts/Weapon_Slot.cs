using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Slot : MonoBehaviour
{
    public Weapon weapon;
    public GameObject curr_gun;
    [SerializeField] GameObject body;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            weapon.Shoot(curr_gun);
        }
    }
    private void Awake()
    {
        weapon.body = body;
    }
}
