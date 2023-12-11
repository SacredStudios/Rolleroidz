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
            if (pv.IsMine)
            {
                pv.Owner.AddScore(collider.gameObject.GetComponent<Point>().value);
                int id = collider.gameObject.GetComponent<PhotonView>().ViewID;
                pv.RPC("DestroyGameObject", RpcTarget.All, id);
            }
        
            
         
        }
        
    }
   
    [PunRPC] public void DestroyGameObject(int id)
    {

        PhotonView.Find(id).gameObject.SetActive(false);   
     
    }
}
