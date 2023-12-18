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
    [SerializeField] CapsuleCollider collider;
    

    public void Death()
    {

        currLoc = player.transform.position;
        death_anim.transform.position = currLoc;
        if (pv.IsMine)
        {
            GameObject death_anim_clone = PhotonNetwork.Instantiate(death_anim.name, currLoc, Quaternion.identity);
            player.GetComponent<PlayerMovement>().enabled = false;
            collider.enabled = false;
            cam.enabled = false;
            player.transform.position = new Vector3(9999, 9999, 9999);
            player.GetComponent<Weapon_Handler>().RemoveSuper();
            Debug.Log(pv.Owner.NickName + " died!");
            GameObject point_clone = PhotonNetwork.Instantiate(point.name, currLoc, Quaternion.identity);
            Point p = point_clone.GetComponent<Point>();
            p.player = player.GetComponent<Player_Health>().last_hit;
            p.pv = player.GetComponent<Player_Health>().last_hit.GetComponent<PhotonView>();
            
            int n = player.GetPhotonView().Owner.GetScore();

            for (int i = 0; i < n; i+=2)
            {

                GameObject point_clone2 = PhotonNetwork.Instantiate(point.name, currLoc, Quaternion.identity);
                Point p2 = point_clone2.GetComponent<Point>();
                p2.player = player.GetComponent<Player_Health>().last_hit;
                p2.pv = player.GetComponent<Player_Health>().last_hit.GetComponent<PhotonView>();
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

    }
  
    void Player_Active()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        cam.enabled = true;
        player.GetComponent<Player_Health>().Add_Health(1000);
        player.GetComponent<PlayerMovement>().enabled = true;
        collider.enabled = true;
        player.SetActive(true);
        player.transform.position = GetFarthestPoint(currLoc);
        if (pv.Owner.GetScore() < 0)
        {
            pv.Owner.SetScore(0);
        }
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

}
