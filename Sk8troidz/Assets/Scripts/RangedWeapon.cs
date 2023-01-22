using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/RangedWeapon", order = 1)]
public class RangedWeapon : Weapon
{
    public GameObject particle_trail1;
    public GameObject particle_trail2;
    Transform spawn_pos;

   // GameObject prefab_instance;

    public override void Shoot(GameObject parent)
    {
        Debug.DrawRay(parent.transform.position, body.transform.forward * 50, Color.green);
        Ray ray = new Ray(parent.transform.position, body.transform.forward * 50);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit)) //Target Acquired
        {
            Debug.Log(hit.collider.name);
        }
        else
        {
            Debug.Log("");
        }
    }
}
