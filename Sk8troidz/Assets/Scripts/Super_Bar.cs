using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class Super_Bar : MonoBehaviourPunCallbacks
{
    [SerializeField] public Slider slider;
    [SerializeField] PhotonView pv;
    [SerializeField] GameObject player;
    [SerializeField] GameObject super_particles;
    public void ChangeAmount(float new_amount)
    {
        Debug.Log("changing amount" + new_amount);
        pv.RPC("ChangeSuperBar", RpcTarget.All, new_amount);       
    }
    [PunRPC] void ChangeSuperBar(float amount)
    {
        slider.value += amount;
        if(slider.value >= 100)
        {
            super_particles.SetActive(true);
        }
        else
        {
            super_particles.SetActive(false);
        }
    }
   
}
