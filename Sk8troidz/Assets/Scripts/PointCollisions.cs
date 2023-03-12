using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;


public class PointCollisions : MonoBehaviourPunCallbacks
{
    [SerializeField] PointTally pt;
    [SerializeField] PhotonView pv;

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Point")
        {
            
            PhotonNetwork.LocalPlayer.AddScore(1);
            Debug.Log(PhotonNetwork.LocalPlayer.GetScore());
            int id = collider.gameObject.GetComponent<PhotonView>().ViewID;
            pv.RPC("DestroyGameObject", RpcTarget.MasterClient,id);
         
        }
        
    }
    [PunRPC] public void DestroyGameObject(int id)
    {
       
            PhotonNetwork.Destroy(PhotonView.Find(id).gameObject);
    
     
    }
}
