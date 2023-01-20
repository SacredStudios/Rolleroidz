using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeColor : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        
    }
}
