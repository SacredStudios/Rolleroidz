using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Slot : MonoBehaviour
{
    public Weapon weapon;
    public GameObject curr_gun;
    [SerializeField] GameObject body; //rename to spine
    [SerializeField] GameObject weapon_loc;

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            
            weapon.Shoot(curr_gun);
        }
    }
    private void Awake()
    {
        weapon.body = body;
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
    }
}
