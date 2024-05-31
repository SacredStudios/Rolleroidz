using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class AI_LookAt : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Text player_name;
    [SerializeField] Text score;
    [SerializeField] GameObject heart;
    public static Camera cam;
    public static PhotonView pv;
    bool check = false;


    void SetText()
    {
        byte team = GetComponent<AI_Handler>().team;
        if (!check)
        {
            check = true;
            player_name.text = "AI Player";
            if (team == 2)
            {
                score.color = new Color(0.48f, 0.16f, 0.42f);
                player_name.color = new Color(0.96f, 0.32f, 0.84f);
            }
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == team)
            {
                heart.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cam != null)
        {
            SetText();
            canvas.transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
            // Player_Name.transform.LookAt(Player_Name.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
        
    }
}

