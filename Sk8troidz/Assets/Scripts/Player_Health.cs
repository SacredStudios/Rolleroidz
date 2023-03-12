using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class Player_Health : MonoBehaviour

{
    [SerializeField] float max_health;
    [SerializeField] float current_health;
    [SerializeField] GameObject death_effect;
    [SerializeField] GameObject parent;
    [SerializeField] Slider health_bar;
    [SerializeField] Slider health_bar_other;
    [SerializeField] PhotonView pv;

    void Start()
    {
        current_health = max_health;    
    }

  
    public void Remove_Health(float amount)
    {
        pv.RPC("ChangeHealth", RpcTarget.All, -1 * amount);

    }
    [PunRPC] void ChangeHealth(float amount)
    {
        current_health += amount;
        health_bar.value = current_health;
        health_bar_other.value = current_health;
        if (current_health <= 0)
        {
            Death();
        }
        else if (current_health > max_health)
        {
            current_health = max_health;
        }

    }

    public void Add_Health(float amount)
    {
        pv.RPC("ChangeHealth", RpcTarget.All, amount);
        

    }


    void Death()
    {
        Respawn rs = GetComponentInParent<Respawn>();
        rs.Death();

     }

    private void OnEnable()
    {
        current_health = 100;
        health_bar.value = current_health;
        health_bar_other.value = current_health;
    }
}
