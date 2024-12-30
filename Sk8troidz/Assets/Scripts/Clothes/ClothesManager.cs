using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ClothesManager : MonoBehaviourPunCallbacks
{
    /*public Clothing Top; //hats are treated differently
    public Clothing Shirt;
    public Clothing Pants;
    public Clothing Shoes;*/

    [SerializeField] GameObject top_obj;
    [SerializeField] GameObject shirt_obj;
    [SerializeField] GameObject pants_obj;
    [SerializeField] GameObject shoes_obj;
    [SerializeField] GameObject sleeveL_obj;
    [SerializeField] GameObject sleeveR_obj;

    [SerializeField] PhotonView pv;
    public static int count = 0;
    GameObject clothes_list;


    void Start()
    {
        clothes_list = GameObject.Find("ClothesList");
        if (pv.IsMine)
        {
            ClothesList cl = clothes_list.GetComponent<ClothesList>();
            if (this.gameObject.tag == "Player")
            {
                
                top_obj.GetComponent<MeshFilter>().mesh = cl.curr_top.mesh;
                shirt_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.mesh;
                pants_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_pants.mesh;
                shoes_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shoes.mesh;
                sleeveL_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.sleeveL_mesh;
                sleeveR_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.sleeveR_mesh;

                top_obj.GetComponent<Renderer>().material = cl.curr_top.material;
                shirt_obj.GetComponent<Renderer>().material = cl.curr_shirt.material;
                pants_obj.GetComponent<Renderer>().material = cl.curr_pants.material;
                shoes_obj.GetComponent<Renderer>().material = cl.curr_shoes.material;
                sleeveL_obj.GetComponent<Renderer>().material = cl.curr_shirt.sleeveL_mat;
                sleeveR_obj.GetComponent<Renderer>().material = cl.curr_shirt.sleeveR_mat;

                pv.RPC("SetTop", RpcTarget.Others, cl.curr_top.name, pv.ViewID);
                pv.RPC("SetShirt", RpcTarget.Others, cl.curr_shirt.name, pv.ViewID);
                pv.RPC("SetPants", RpcTarget.Others, cl.curr_pants.name, pv.ViewID);
                pv.RPC("SetShoes", RpcTarget.Others, cl.curr_shoes.name, pv.ViewID);
                //change to other
                //shoes_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shoes.mesh;
            }
            else
            {
                Clothing top = cl.all_tops[Random.Range(0, 3)];
                Clothing shirt = cl.all_shirts[Random.Range(0, 4)];
                Clothing pants = cl.all_pants[Random.Range(0, 3)];
                Clothing shoes = cl.all_shoes[Random.Range(0, 3)];
                top_obj.GetComponent<MeshFilter>().mesh = top.mesh;
                shirt_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = shirt.mesh;
                pants_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = pants.mesh;
                shoes_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = shoes.mesh;
                sleeveL_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = shirt.sleeveL_mesh;
                sleeveR_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = shirt.sleeveR_mesh;

                top_obj.GetComponent<Renderer>().material = top.material;
                shirt_obj.GetComponent<Renderer>().material = shirt.material;
                pants_obj.GetComponent<Renderer>().material = pants.material;
                shoes_obj.GetComponent<Renderer>().material = shoes.material;
                sleeveL_obj.GetComponent<Renderer>().material = shirt.sleeveL_mat;
                sleeveR_obj.GetComponent<Renderer>().material = shirt.sleeveR_mat;

                pv.RPC("SetTop", RpcTarget.Others, top.name, pv.ViewID);
                pv.RPC("SetShirt", RpcTarget.Others, shirt.name, pv.ViewID);
                pv.RPC("SetPants", RpcTarget.Others, pants.name, pv.ViewID);
                pv.RPC("SetShoes", RpcTarget.Others, shoes.name, pv.ViewID);
            }
        }

    }

[PunRPC] void SetTop(string currname, int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;
        GameObject top = player.GetComponent<ClothesManager>().top_obj;
        Debug.Log(PhotonView.Find(viewID).Owner.NickName);
        foreach (Clothing c in clothes_list.GetComponent<ClothesList>().all_tops)
        {
 //Photon Hashtable might be more efficient.
            if (c.name == currname)
            {
                Debug.Log(c.name);
                top.GetComponent<MeshFilter>().mesh = c.mesh;
                top.GetComponent<Renderer>().material = c.material;
            }
        }
    }

    [PunRPC]
    void SetShirt(string currname, int viewID)
    {
        GameObject clothes_list = GameObject.Find("ClothesList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        GameObject shirt = player.GetComponent<ClothesManager>().shirt_obj;
        GameObject sleeveL = player.GetComponent<ClothesManager>().sleeveL_obj;
        GameObject sleeveR = player.GetComponent<ClothesManager>().sleeveR_obj;
        foreach (Clothing c in clothes_list.GetComponent<ClothesList>().all_shirts)
        {//Photon Hashtable might be more efficient.
            if (c.name == currname)
            {
                Debug.Log(c.name);
                shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = c.mesh;
                shirt.GetComponent<Renderer>().material = c.material;
                sleeveL.GetComponent<SkinnedMeshRenderer>().sharedMesh = c.sleeveL_mesh;
                sleeveL.GetComponent<Renderer>().material = c.sleeveL_mat;
                sleeveR.GetComponent<SkinnedMeshRenderer>().sharedMesh = c.sleeveR_mesh;
                sleeveR.GetComponent<Renderer>().material = c.sleeveR_mat;
            }
        }
    }
    [PunRPC]
    void SetPants(string currname, int viewID)
    {
        GameObject clothes_list = GameObject.Find("ClothesList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        GameObject pants = player.GetComponent<ClothesManager>().pants_obj;

        foreach (Clothing c in clothes_list.GetComponent<ClothesList>().all_pants)
        {//Photon Hashtable might be more efficient.
            if (c.name == currname)
            {
                Debug.Log(c.name);
                pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = c.mesh;
                pants.GetComponent<Renderer>().material = c.material;
            }
        }
    }
    [PunRPC]
    void SetShoes(string currname, int viewID)
    {
        GameObject clothes_list = GameObject.Find("ClothesList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        GameObject shoes = player.GetComponent<ClothesManager>().shoes_obj;

        foreach (Clothing c in clothes_list.GetComponent<ClothesList>().all_shoes)
        {//Photon Hashtable might be more efficient.
            if (c.name == currname)
            {
                Debug.Log(c.name);
                shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = c.mesh;
                shoes.GetComponent<Renderer>().material = c.material;
            }
        } //MAKE setshoes here
    }

}
