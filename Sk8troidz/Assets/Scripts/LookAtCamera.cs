using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject Player_Assets;
    [SerializeField] PhotonView pv;
    [SerializeField] GameObject Player_Name;
    public static Camera cam;
    

    void Start()
    {
        if(pv.IsMine)
        {
            Player_Name.GetComponent<Text>().text = PhotonNetwork.NickName;
            canvas.SetActive(false);
            cam = Player_Assets.GetComponentInChildren<Camera>();
          
        }
        else
        {
            Player_Name.GetComponent<Text>().text = pv.Owner.NickName;
            
            
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
