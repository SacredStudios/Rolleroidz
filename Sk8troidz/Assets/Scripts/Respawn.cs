using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class Respawn : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float respawn_time;
    [SerializeField] GameObject death_anim;
    [SerializeField] GameObject death_head;
    [SerializeField] GameObject point;
    [SerializeField] PhotonView pv;
    public void Death()
    {

        player.SetActive(false);

        GameObject death_anim_clone = Instantiate(death_anim, player.transform.position, Quaternion.identity);
        death_anim_clone.SetActive(true);
        if (pv.IsMine)
        {
            Debug.Log(pv.Owner.NickName + " died!");
            GameObject point_clone = PhotonNetwork.Instantiate(point.name, death_anim_clone.transform.position, Quaternion.identity);
            point_clone.SetActive(true);
            player.GetPhotonView().Owner.AddScore(-1);
            int n = player.GetPhotonView().Owner.GetScore();

            for (int i = 0; i < n / 2; i++)
            {

                GameObject point_clone2 = PhotonNetwork.Instantiate(point.name, death_anim_clone.transform.position, Quaternion.identity);
                point_clone2.SetActive(true);
                player.GetPhotonView().Owner.AddScore(-1);
            }

            if (pv.Owner.GetScore() < 0)
            {
                pv.Owner.SetScore(0);
            }
        }

        GameObject death_head_clone = Instantiate(death_head, player.transform.position, Quaternion.identity);
        death_head_clone.SetActive(true);
       
        Invoke("Player_Active",  respawn_time);
}
    void Player_Active()
    {
        player.SetActive(true);
        if (pv.Owner.GetScore() < 0)
        {
            pv.Owner.SetScore(0);
        }
    }

}
