using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Point : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] int y_val; //set pv to ismine instead of masterclient

    public PhotonView pv;
    public GameObject player;
    void Start()
    { if (pv.IsMine)
        {
            Debug.Log(player);
            int x = Random.Range(min, max);
            int z = Random.Range(min, max);
            rb.AddForce(new Vector3(x, y_val, z));
        }
    }

    
}
