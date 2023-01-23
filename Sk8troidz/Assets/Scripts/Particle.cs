using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float lifespan;
    void OnEnable()
    {
        Invoke("Destroy_Object", lifespan);
    }

    // Update is called once per frame
    void Destroy_Object()
    {
        Destroy(this.gameObject);   
    }
}
