using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float respawn_time;
    [SerializeField] GameObject death_anim;
    [SerializeField] GameObject death_head;
    
    public void Death()
{
        //Add death anim. I was thinking maybe everything explodes and a head by itself spawns with the eye pupil rotating around- Past Jessie

        player.SetActive(false);
        GameObject death_anim_clone = Instantiate(death_anim, player.transform.position, Quaternion.identity);
        death_anim_clone.SetActive(true);
        GameObject death_head_clone = Instantiate(death_head, player.transform.position, Quaternion.identity);
        death_head_clone.SetActive(true);
        death_head.GetComponent<Rigidbody>().AddForce(0,100,0);
        Invoke("Player_Active",  respawn_time);
}
    void Player_Active()
    {
        player.SetActive(true);
    }

}
