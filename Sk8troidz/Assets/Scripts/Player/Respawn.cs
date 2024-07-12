using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cinemachine;
using UnityEngine.AI;

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
    [SerializeField] GameObject vcam;

    [SerializeField] GameObject whoShotYou;
    [SerializeField] GameObject RespawnCircle;
    GameObject respawn_cam;
    [SerializeField] GameObject regular_cam;
    [SerializeField] GameObject respawn_btn;
    [SerializeField] GameObject trick_system;
    private void Start()
    {
        respawn_cam = GameObject.Find("RespawnCam");
    }
    public void Death(int id)
    {
        //test if AI leaves if masterclient leaves
        Debug.Log("player died");
        Cursor.lockState = CursorLockMode.None;
        currLoc = player.transform.position;
        death_anim.transform.position = currLoc;
        if (pv.IsMine)
        {
            
            GameObject death_anim_clone = PhotonNetwork.Instantiate(death_anim.name, currLoc, Quaternion.identity);
            
            collider.enabled = false;
            
            
            if (player.tag == "Player")
            {
                cam.enabled = false;
                player.transform.position = new Vector3(9999, 9999, 9999);
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<Weapon_Handler>().RemoveSuper();
                trick_system.SetActive(false);
                if (pv.Owner.GetScore() < 0)
                {
                    pv.Owner.SetScore(0);
                }
                else if (pv.Owner.GetScore() % 2 != 0)
                {
                    pv.Owner.AddScore(1);
                }

                pv.Owner.SetScore(pv.Owner.GetScore() / 2);
            }
            else
            {
                player.GetComponent<AI_Movement>().enabled = false;
                player.GetComponent<AI_Handler>().DivideScore();
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.transform.position = new Vector3(9999, 9999, 9999);

            }
            
            Invoke("Respawn_Screen", respawn_time);
        }

    }
  
    void Respawn_Screen()
    {
        Invoke("Respawn_Player", 10f);
        
        player.GetComponent<Player_Health>().Add_Health(1000);
        
        //player.GetComponent<PlayerMovement>().enabled = true;
        collider.enabled = true;
        //player.SetActive(true);
        if (player.tag == "Player")
        {
            cam.enabled = true;
            vcam.GetComponent<CinemachineVirtualCamera>().Follow = respawn_cam.transform;
            respawn_btn.SetActive(true);
            RespawnCircle.SetActive(true);

            //player.transform.position = GetFarthestPoint(currLoc);
            if (pv.Owner.GetScore() < 0)
            {
                pv.Owner.SetScore(0);
            }
        }
        else
        {     
            player.GetComponent<AI_Movement>().enabled = true;
            player.transform.position = respawn_points[Random.Range(0, respawn_points.Count)];
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

    }
    public void Respawn_Player()
    {
        if (RespawnCircle.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            collider.enabled = true;
            vcam.GetComponent<CinemachineVirtualCamera>().Follow = regular_cam.transform;
            player.transform.position = RespawnCircle.transform.position + new Vector3(0, 10f, 0);
            player.GetComponent<PlayerMovement>().enabled = true;
            trick_system.SetActive(true);
            RespawnCircle.SetActive(false);
            respawn_btn.SetActive(false);
        }
    }
    Vector3 GetFarthestPoint(Vector3 pos)
    {
        Vector3 result = new Vector3();
        float distance = 0;
        for (int i = 0; i < respawn_points.Count; i++)
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
