using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Selector : MonoBehaviour
{
    public static Weapon curr_weapon;
    [SerializeField] string weapon_name;
    public GameObject weapon_list = null;
    public void ChangeWeapon(Weapon weapon)
    {
        if (weapon_list.GetComponent<Weapon_List>().my_weapon_list.Contains(weapon))
        {
        curr_weapon = weapon;
        weapon_name = weapon.name;
        }
    }
    

}
