
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;


public class Weapon : ScriptableObject
{
    public Texture icon;
    public string weapon_name;
    [TextArea(15,20)]
    public string weapon_description;
    public float weapon_delay;
    public float damage;
    public int attack_cost;
    public float knockback;
    public Vector3 offset;
    public PhotonView pv;
    public float range;
    public int max_ammo;
    public int ammo;
    public bool isSuper;
    public AudioClip sound;
    [SerializeField] GameObject point;
    public GameObject player;
    public Weapon super;

    public GameObject instance;
    //Add var for sound

    


   public virtual void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos) { }

    public void SpawnPoint(GameObject deadPlayer)
    {
       if (deadPlayer.GetComponent<Respawn>().IsDead == false) {
            deadPlayer.GetComponent<Respawn>().IsDead = true;
            int n = deadPlayer.GetPhotonView().Owner.GetScore();
            Debug.Log(n);
            GameObject point_clone = PhotonNetwork.Instantiate(point.name, deadPlayer.transform.position, Quaternion.identity);
            point_clone.GetComponent<Point>().value = n / 2 + 1;
        }
    }

   

   
}
