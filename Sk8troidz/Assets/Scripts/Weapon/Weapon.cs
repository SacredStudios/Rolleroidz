
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
    public float min_distance = 2.5f;
    public GameObject death_effect;




    public virtual void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos) { }

    public void SpawnCoin(GameObject dead_player, Vector3 pos) //change the name of this
    {
        Photon.Realtime.Player player_photon = player.GetComponent<PhotonView>().Owner;
        Photon.Realtime.Player dead_player_photon = dead_player.GetComponent<PhotonView>().Owner;
        if (player.tag == "AI_Player")
        {
            //TODO: Change this to Photon.Pun function
            player.GetComponent<AI_Handler>().AddScore((dead_player.GetComponent<Team_Handler>().GetScore() / 2) + 2);
        }
        else
        {
            player.GetComponentInChildren<Trick_System>().Add_To_Multiplier();
            player_photon.AddScore((dead_player.GetComponent<Team_Handler>().GetScore() / 2) + 2);
        }
        //pv.RPC("PrintKO", RpcTarget.All, player_photon.NickName, dead_player_photon.NickName); 
    }
    [PunRPC] void PrintKO(string player, string dead_player)
    {
        Debug.Log(player + " KO'd " + dead_player);
    }
   


}
