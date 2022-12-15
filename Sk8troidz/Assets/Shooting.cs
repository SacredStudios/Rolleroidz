using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject head;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }
    public void Shoot()
    {
        Debug.DrawRay(head.transform.position, head.transform.forward*50, Color.green);
     // if(Physics.Raycast(ray, out hit))
       // {
      //      Transform objectHit = hit.transform;
      //      Debug.Log(hit.transform.name);
       // }
       // Physics.Raycast(head.transform.position, head.transform.rotation, 100);
    }

}
