using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes_Btn : MonoBehaviour
{
    public Clothing clothing;
    public ClothesList list;
    public void Is_Pressed()
    {
        list.GetComponent<ClothesList>().ChangeClothes(clothing);

    }
}
