using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeExplosionSound : MonoBehaviour
{
    [SerializeField] AudioSource sound;
    void Start()
    {
        sound.Play();
    }

    
}
