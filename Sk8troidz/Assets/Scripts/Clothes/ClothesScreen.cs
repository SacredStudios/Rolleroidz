using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesScreen : MonoBehaviour
{
    [SerializeField] GameObject clothes_btn;
    [SerializeField] GameObject top_panel;
    [SerializeField] GameObject shirt_panel;
    [SerializeField] GameObject pants_panel;
    [SerializeField] GameObject shoes_panel;
    void Start()
    {
        GameObject clothes_list = GameObject.Find("ClothesList");
        foreach (Clothing top in clothes_list.GetComponent<ClothesList>().my_tops)
        {
            GameObject btn = Instantiate(clothes_btn, top_panel.transform);
            btn.SetActive(true);
            btn.GetComponent<Clothes_Btn>().clothing = top;
            btn.GetComponent<Clothes_Btn>().list = clothes_list.GetComponent<ClothesList>();
            btn.GetComponentInChildren<RawImage>().texture = top.icon;
            top_panel.transform.position = new Vector3(top_panel.transform.position.x, btn.transform.position.y, top_panel.transform.position.z);
        }
        foreach (Clothing shirt in clothes_list.GetComponent<ClothesList>().my_shirts)
        {
            GameObject btn = Instantiate(clothes_btn, shirt_panel.transform);
            btn.SetActive(true);
            btn.GetComponent<Clothes_Btn>().clothing = shirt;
            btn.GetComponent<Clothes_Btn>().list = clothes_list.GetComponent<ClothesList>();
            btn.GetComponentInChildren<RawImage>().texture = shirt.icon;
            shirt_panel.transform.position = new Vector3(shirt_panel.transform.position.x, btn.transform.position.y, shirt_panel.transform.position.z);
        }
        foreach (Clothing pants in clothes_list.GetComponent<ClothesList>().my_pants)
        {
            GameObject btn = Instantiate(clothes_btn, pants_panel.transform);
            btn.SetActive(true);
            btn.GetComponent<Clothes_Btn>().clothing = pants;
            btn.GetComponent<Clothes_Btn>().list = clothes_list.GetComponent<ClothesList>();
            btn.GetComponentInChildren<RawImage>().texture = pants.icon;
            pants_panel.transform.position = new Vector3(pants_panel.transform.position.x, btn.transform.position.y, pants_panel.transform.position.z);
        }
        foreach (Clothing shoes in clothes_list.GetComponent<ClothesList>().my_shoes)
        {
            GameObject btn = Instantiate(clothes_btn, shoes_panel.transform);
            btn.SetActive(true);
            btn.GetComponent<Clothes_Btn>().clothing = shoes;
            btn.GetComponent<Clothes_Btn>().list = clothes_list.GetComponent<ClothesList>();
            btn.GetComponentInChildren<RawImage>().texture = shoes.icon;
            shoes_panel.transform.position = new Vector3(shoes_panel.transform.position.x, btn.transform.position.y, shoes_panel.transform.position.z);
        }
    }

   
}
