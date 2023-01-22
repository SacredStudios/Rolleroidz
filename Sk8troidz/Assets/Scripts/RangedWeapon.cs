using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/RangedWeapon", order = 1)]
public class RangedWeapon : Weapon
{
    public GameObject instance;
    Transform spawn_pos;

    GameObject prefab_instance;

    public override void Shoot(GameObject parent)
    {
       
    }
}
