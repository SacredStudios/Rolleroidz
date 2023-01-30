using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour

{
    [SerializeField] int max_health;
    public int current_health;
 
    void Start()
    {
        current_health = max_health; 
    }

  
    public void Remove_Health(int amount)
    {
        current_health -= amount;
        if(current_health < 0)
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
        Destroy(this.gameObject);
        //Add death anim. I was thinking maybe everything explodes and a head by itself spawns with the eye pupil rotating around- Past Jessie
    }
}
