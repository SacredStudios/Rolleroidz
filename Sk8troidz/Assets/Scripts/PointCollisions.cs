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
                pv.Owner.AddScore(1);
            }
        
            int id = collider.gameObject.GetComponent<PhotonView>().ViewID;
            pv.RPC("DestroyGameObject", RpcTarget.All,id);
         
        }
        
    }
    private void Update()
    {
        if (pv.IsMine)
            Debug.Log(pv.Owner.GetScore());
    }
    [PunRPC] public void DestroyGameObject(int id)
    {
       
        PhotonView.Find(id).gameObject.SetActive(false);
      
    
     
    }
}
