using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour

{
    [SerializeField] int max_health;
    [SerializeField] int current_health;
    [SerializeField] GameObject death_effect;
    [SerializeField] GameObject parent;
 
    void Start()
    {
        current_health = max_health;    
    }

  
    public void Remove_Health(int amount)
    {
        current_health -= amount;
        if(current_health <= 0)
        {
            Death();
        }
    }

    public void Add_Health(int amount)
    {
        current_health += amount;
        if (current_health > max_health)
        {
            current_health = max_health;
        }
    }


    void Death()
    {
        Respawn rs = GetComponentInParent<Respawn>();
        rs.Death();

     }

    private void OnEnable()
    {
        current_health = 100;
    }
}
