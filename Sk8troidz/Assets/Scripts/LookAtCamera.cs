using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class LookAtCamera : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject Player_Assets;
    [SerializeField] PhotonView pv;
    [SerializeField] Text player_name;
    [SerializeField] Text score;
    public static Camera cam;
    

    void Start()
    {
        if(pv.IsMine)
        {
            player_name.text = PhotonNetwork.NickName;
            canvas.SetActive(false);
            cam = Player_Assets.GetComponentInChildren<Camera>();
          
        }
        else
        {
            player_name.text = pv.Owner.NickName;
            Debug.Log(pv.Owner.GetPhotonTeam().Code);
            if (pv.Owner.GetPhotonTeam().Code == 2)
            {
                score.color = new Color(0.48f, 0.16f, 0.42f);
                player_name.color = new Color(0.96f, 0.32f, 0.84f);
            }
           

        }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
       if (cam != null)
        {
            
            canvas.transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
           // Player_Name.transform.LookAt(Player_Name.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        } 
    }
}
