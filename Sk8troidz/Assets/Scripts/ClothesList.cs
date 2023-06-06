using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClothesList : MonoBehaviour
{
    //public List<Clothing> all_clothes;
    public List<Clothing> my_tops;
    public List<Clothing> my_shirts;
    public List<Clothing> my_pants;
    public List<Clothing> my_shoes;

    public List<Clothing> all_tops;
    public List<Clothing> all_shirts;
    public List<Clothing> all_pants;
    public List<Clothing> all_shoes;

    public Clothing curr_top;
    public Clothing curr_shirt;
    public Clothing curr_pants;
    public Clothing curr_shoes;

    [SerializeField] Clothing def_top; //you can delete this when you make playerprefs
    [SerializeField] Clothing def_shirt;
    public Clothing def_pants;
    public Clothing def_shoes;
    public GameObject sk8troid_menu;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        sk8troid_menu = GameObject.Find("Sk8troid(MENUVERSION)");
        ChangeClothes(def_top);
        ChangeClothes(def_shirt);
        ChangeClothes(def_pants);
        ChangeClothes(def_shoes);

    }

    public void ChangeClothes(Clothing clothes)
    {
        sk8troid_menu.GetComponent<Clothes_Dummy>().ChangeClothes(clothes);
        Debug.Log(clothes.name);
        switch (clothes.type)
        {
            case Clothing.Type.Top:
                curr_top = clothes;
                break;
            case Clothing.Type.Shirt:
                curr_shirt = clothes;
                break;
            case Clothing.Type.Pants:
                curr_pants = clothes;
                break;
            case Clothing.Type.Shoes:
                curr_shoes = clothes;
                break;
        }
    }
}
