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



    void Start()
    {
        GameObject list = GameObject.Find("ClothesList");
        ClothesList cl = list.GetComponent<ClothesList>();
        top_obj.GetComponent<MeshFilter>().mesh = cl.curr_top.mesh;
        shirt_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.mesh;
        pants_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_pants.mesh;
        sleeveL_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.sleeveL_mesh;
        sleeveR_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shirt.sleeveR_mesh;

        top_obj.GetComponent<Renderer>().material = cl.curr_top.material;
        shirt_obj.GetComponent<Renderer>().material = cl.curr_shirt.material;
        pants_obj.GetComponent<Renderer>().material = cl.curr_pants.material;
        sleeveL_obj.GetComponent<Renderer>().material = cl.curr_shirt.sleeveL_mat;
        sleeveR_obj.GetComponent<Renderer>().material = cl.curr_shirt.sleeveR_mat;

        //shoes_obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = cl.curr_shoes.mesh;
    }

    
}
