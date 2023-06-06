using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes_Dummy : MonoBehaviour
{
    [SerializeField] GameObject top_obj;
    [SerializeField] GameObject shirt_obj;
    [SerializeField] GameObject pants_obj;
    [SerializeField] GameObject shoes_obj;
    [SerializeField] GameObject sleeveL_obj;
    [SerializeField] GameObject sleeveR_obj;
    [SerializeField] GameObject weapon_loc;
    [SerializeField] GameObject curr_weapon;

    //This is for the sk8troid on the menu, so it changes clothes after each change
    private void Awake()
    {
        GameObject ClothesList = GameObject.Find("ClothesList");
        ClothesList cl = ClothesList.GetComponent<ClothesList>();
        cl.sk8troid_menu = this.gameObject;
        cl.ChangeClothes(cl.curr_top);
        cl.ChangeClothes(cl.curr_shirt);
        cl.ChangeClothes(cl.curr_pants);
        cl.ChangeClothes(cl.curr_shoes);
    }
    public void ChangeClothes(Clothing clothes)
    {
        switch (clothes.type)
        {                
            case Clothing.Type.Top:
                top_obj.GetComponent<MeshFilter>().mesh = clothes.mesh;
                top_obj.GetComponent<Renderer>().material = clothes.material;
                break;
            case Clothing.Type.Shirt:
                shirt_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = clothes.mesh;
                sleeveL_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = clothes.sleeveL_mesh;
                sleeveR_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = clothes.sleeveR_mesh;

                shirt_obj.GetComponent<Renderer>().material = clothes.material;
                sleeveL_obj.GetComponent<Renderer>().material = clothes.sleeveL_mat;
                sleeveR_obj.GetComponent<Renderer>().material = clothes.sleeveR_mat;
                break;
            case Clothing.Type.Pants:
                pants_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = clothes.mesh;
                pants_obj.GetComponent<Renderer>().material = clothes.material;
                break;
            case Clothing.Type.Shoes:
                shoes_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = clothes.mesh;
                shoes_obj.GetComponent<Renderer>().material = clothes.material;
                break;
        }
    }
    public void ChangeWeapon(Weapon weapon)
    {
        Destroy(curr_weapon);
        curr_weapon = Instantiate(weapon.instance, weapon_loc.transform);
    }
}
