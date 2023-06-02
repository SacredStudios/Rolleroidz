using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesRandomizer : MonoBehaviour
{
    [SerializeField] GameObject clotheslist;
    void Start()
    {
        ClothesList cl = clotheslist.GetComponent<ClothesList>();
        cl.ChangeClothes(cl.all_tops[Random.Range(0, 3)]);
        cl.ChangeClothes(cl.all_shirts[Random.Range(0, 2)]);
        cl.ChangeClothes(cl.def_pants);
        cl.ChangeClothes(cl.def_shoes);

    }

}
