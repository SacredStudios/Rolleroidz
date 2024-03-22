
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
    public float weight;

    public GameObject player;
    public Weapon super;

    public GameObject instance;
    public float shake;
    //Add var for sound
    public GameObject coin;




    public virtual void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos) { }

    public void SpawnCoin(GameObject dead_player, Vector3 pos)
    {
        Debug.Log(player.GetComponent<PhotonView>().Owner.NickName);
        Debug.Log(dead_player.GetComponent<PhotonView>().Owner.NickName);

        Debug.Log("This should happen only once");
        GameObject coin_clone = PhotonNetwork.Instantiate(coin.name, pos, Quaternion.identity);
        Debug.Log(coin_clone.name);
        coin_clone.GetComponent<Point>().player = this.player;
        coin_clone.GetComponent<Point>().dead_player = dead_player;
        coin_clone.GetComponent<Point>().value = (dead_player.GetComponent<PhotonView>().Owner.GetScore() / 2) + 1;


    }


}
