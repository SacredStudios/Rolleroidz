using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class Super_Bar : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider slider;
    [SerializeField] PhotonView pv;
    [SerializeField] GameObject player;
    public void ChangeAmount(float new_amount)
    {
        Debug.Log("changing amount" + new_amount);
        pv.RPC("ChangeSuperBar", RpcTarget.All);       
    }
    [PunRPC] void ChangeSuperBar()
    {
        slider.value += 15;
    }
   
}
