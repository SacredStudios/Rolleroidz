using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/RangedWeapon", order = 1)]
public class RangedWeapon : Weapon
{
    public GameObject particle_trail;
    public GameObject particle_explosion;
    public GameObject impact_explosion;
    public float range;

   // MAKE GAME LIKE NSMBDS MULTIPLAYER
  
    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
      //  Debug.DrawRay(parent.transform.position, particle_pos.transform.up * range, Color.green); //chage this to capsulecast
        Ray ray = new Ray(parent.transform.position, particle_pos.transform.up * range); ;
        RaycastHit hit = new RaycastHit();
        Instantiate(particle_trail, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);
        
        if (Physics.Raycast(ray, out hit)) //Target Acquired
        {
           // Debug.Log(hit.collider.name);
            Instantiate(impact_explosion, hit.point, Quaternion.identity);
            if(hit.collider.tag == "Player")
            {
               // Debug.Log(hit.collider.name);
 
                hit.collider.GetComponent<Player_Health>().Remove_Health(damage);
                
            }
            else if (hit.collider.tag == "Player_Head")
            {
                hit.collider.GetComponentInParent<Player_Health>().Remove_Health(damage*1.5f);
                Debug.Log("HeadShot");
            }
          
        }
       
    }
}
