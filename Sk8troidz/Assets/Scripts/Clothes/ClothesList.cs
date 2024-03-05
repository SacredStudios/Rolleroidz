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
        
        if(PlayerPrefs.HasKey("top")) {
            LoadClothes(PlayerPrefs.GetString("top"), Clothing.Type.Top);
        }
        else
        {
            LoadClothes(my_tops[1].name, Clothing.Type.Top);
        }
        if (PlayerPrefs.HasKey("shirt"))
        {
            LoadClothes(PlayerPrefs.GetString("shirt"), Clothing.Type.Shirt);
        }
        else
        {
            LoadClothes(my_shirts[2].name, Clothing.Type.Shirt);
        }
        if (PlayerPrefs.HasKey("pants"))
        {
            LoadClothes(PlayerPrefs.GetString("pants"), Clothing.Type.Pants);
        }
        else
        {
            LoadClothes(my_pants[1].name, Clothing.Type.Pants);
        }
        if (PlayerPrefs.HasKey("shoes"))
        {
            LoadClothes(PlayerPrefs.GetString("shoes"), Clothing.Type.Shoes);
        }
        else
        {
            LoadClothes(my_shoes[1].name, Clothing.Type.Shoes);
        }
    }

    public void LoadClothes(string name, Clothing.Type type)
    {
        switch (type)
        {
            case Clothing.Type.Top: 
                foreach(Clothing clothing in my_tops) //would be better if I used a hashmap
                {
                    
                    if(clothing.name == name)
                    {
                        Debug.Log(name + clothing.name);
                        ChangeClothes(clothing);
                    }
                }
                break;
            case Clothing.Type.Shirt:
                foreach (Clothing clothing in my_shirts)
                {

                    if (clothing.name == name)
                    {
                        Debug.Log(name + clothing.name);
                        ChangeClothes(clothing);
                    }
                }
                break;
            case Clothing.Type.Pants:
                foreach (Clothing clothing in my_pants)
                {

                    if (clothing.name == name)
                    {
                        Debug.Log(name + clothing.name);
                        ChangeClothes(clothing);
                    }
                }
                break;
            case Clothing.Type.Shoes:
                foreach (Clothing clothing in my_shoes)
                {

                    if (clothing.name == name)
                    {
                        Debug.Log(name + clothing.name);
                        ChangeClothes(clothing);
                    }
                }
                break;
        }
    }

    public void ChangeClothes(Clothing clothes)
    {
        sk8troid_menu.GetComponent<Clothes_Dummy>().ChangeClothes(clothes);
        switch (clothes.type)
        {
            case Clothing.Type.Top:
                curr_top = clothes;
                PlayerPrefs.SetString("top", clothes.name);
                PlayerPrefs.Save();
                break;
            case Clothing.Type.Shirt:
                curr_shirt = clothes;
                PlayerPrefs.SetString("shirt", clothes.name);
                PlayerPrefs.Save();
                break;
            case Clothing.Type.Pants:
                curr_pants = clothes;
                PlayerPrefs.SetString("pants", clothes.name);
                PlayerPrefs.Save();
                break;
            case Clothing.Type.Shoes:
                curr_shoes = clothes;
                PlayerPrefs.SetString("shoes", clothes.name);
                PlayerPrefs.Save();
                break;
        }
    }
}
