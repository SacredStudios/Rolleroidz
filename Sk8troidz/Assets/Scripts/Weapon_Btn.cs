using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Btn : MonoBehaviour
{
    public Weapon weapon;
    public Weapon_List list;
    public void Is_Pressed()
    {
        list.GetComponent<Weapon_List>().ChangeWeapon(weapon);

    }
}
