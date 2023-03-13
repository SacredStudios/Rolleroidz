using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;


public class PointCollisions : MonoBehaviourPunCallbacks
{

    [SerializeField] PhotonView pv;

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Point")
        {
           // if (pv.IsMine)
           // {
                collider.gameObject.GetPhotonView().Owner.AddScore(1);
           // }
            Debug.Log(PhotonNetwork.LocalPlayer.GetScore());
            int id = collider.gameObject.GetComponent<PhotonView>().ViewID;
            pv.RPC("DestroyGameObject", RpcTarget.All,id);
         
        }
        
    }
    [PunRPC] public void DestroyGameObject(int id)
    {
       
        Destroy(PhotonView.Find(id).gameObject);
      
    
     
    }
}
