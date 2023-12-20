using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Point : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] int y_val;
    public GameObject player;

    private void Start()
    {
        int x = Random.Range(min, max);
        int z = Random.Range(min, max);
        rb.AddForce(new Vector3(x, y_val, z));
    }
    void Update()
    { if (player != null)
        {
            Debug.Log(player);
        }
        
    }

    
}
