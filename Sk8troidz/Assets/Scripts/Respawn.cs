using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cinemachine;
public class Respawn : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float respawn_time;
    [SerializeField] GameObject death_anim;
    [SerializeField] GameObject death_head;
    [SerializeField] GameObject point;
    [SerializeField] PhotonView pv;
    public List<Vector3> respawn_points;
    [SerializeField] Vector3 currLoc;
    [SerializeField] CinemachineBrain cam;

    public void Death()
    {
        GameObject death_anim_clone = Instantiate(death_anim, player.transform.position, Quaternion.identity);

        death_anim_clone.SetActive(true);
        currLoc = player.transform.position;
        GameObject point_clone = PhotonNetwork.Instantiate(point.name, death_anim_clone.transform.position, Quaternion.identity);
        point_clone.SetActive(true);


        //Debug.Log(player.transform.position);


        if (pv.IsMine)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            cam.enabled = false;
            player.transform.position = new Vector3(9999, 9999, 9999);
            player.GetComponent<Weapon_Handler>().RemoveSuper();
            Debug.Log(pv.Owner.NickName + " died!");
            GameObject point_clone = PhotonNetwork.Instantiate(point.name, death_anim_clone.transform.position, Quaternion.identity);
            point_clone.SetActive(true);
            int n = player.GetPhotonView().Owner.GetScore();

            for (int i = 0; i < n / 2; i++)
            {

                GameObject point_clone2 = PhotonNetwork.Instantiate(point.name, death_anim_clone.transform.position, Quaternion.identity);
                point_clone2.SetActive(true);
            }

            if (pv.Owner.GetScore() < 0)
            {

                pv.Owner.SetScore(0);
            }
            else if (pv.Owner.GetScore() % 2 != 0)
            {
                pv.Owner.AddScore(1);
            }
            pv.Owner.SetScore(pv.Owner.GetScore() / 2);
            Invoke("Player_Active", respawn_time);
        }

        GameObject death_head_clone = Instantiate(death_head, player.transform.position, Quaternion.identity);
        death_head_clone.SetActive(true);

    }
  
    void Player_Active()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        cam.enabled = true;
        player.GetComponent<Player_Health>().Add_Health(100);

        player.SetActive(true);
        player.transform.position = GetFarthestPoint(currLoc);
        if (pv.Owner.GetScore() < 0)
        {
            pv.Owner.SetScore(0);
        }
        Invoke("GlitchCheck", 1f);
    }
    Vector3 GetFarthestPoint(Vector3 pos)
    {
        Vector3 result = new Vector3();
        float distance = 0;
        for (int i = 0; i< respawn_points.Count; i++)
        {
            float temp = Mathf.Sqrt(
                  Mathf.Pow(respawn_points[i].x - pos.x, 2) //distance formula
                + Mathf.Pow(respawn_points[i].y - pos.y, 2)
                + Mathf.Pow(respawn_points[i].z - pos.z, 2));
            if (temp > distance) 
            {
                distance = temp;
                result = respawn_points[i];
            }
        }
        return result;
    }
    public void GlitchCheck()
    {
        Debug.Log("check");
        this.gameObject.SetActive(true);
    }
}
