using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] int y_val;
    void Start()
    {
        int x = Random.Range(min, max);
        int z = Random.Range(min, max);
        rb.AddForce(new Vector3(x, y_val, z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
