using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat_Slider : MonoBehaviour
{
    [SerializeField] GameObject range_slider;
    [SerializeField] GameObject damage_slider;
    [SerializeField] GameObject speed_slider;
    [SerializeField] GameObject attackcost_slider;
    [SerializeField] GameObject weapon_name;
    [SerializeField] GameObject description;
    void Awake()
    {
        GameObject wl = GameObject.Find("WeaponList");
        Weapon_List list = wl.GetComponent<Weapon_List>();
        list.range_slider = this.range_slider;
        list.damage_slider = this.damage_slider;
        list.speed_slider = this.speed_slider;
        list.attackcost_slider = this.attackcost_slider;
        list.weapon_name = this.weapon_name;
        list.description = this.description;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
