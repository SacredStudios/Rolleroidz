using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_Screen : MonoBehaviour
{
    [SerializeField] GameObject Weapon_Btn;
    void Start()
    {
        GameObject weapon_list = GameObject.Find("WeaponList");
        foreach (Weapon w in weapon_list.GetComponent<Weapon_List>().my_weapon_list)
        {
            GameObject btn = Instantiate(Weapon_Btn,this.gameObject.transform);
            btn.SetActive(true);
            Debug.Log("instantiatingbtn");
            btn.GetComponent<Weapon_Btn>().weapon = w;
            btn.GetComponent<Weapon_Btn>().list = weapon_list.GetComponent<Weapon_List>();
            btn.GetComponentInChildren<RawImage>().texture = w.icon;
            
        }
        }

    

}
