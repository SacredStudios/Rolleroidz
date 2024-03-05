using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTranslation : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector3 starting_loc;
    [SerializeField] Vector3 ending_loc;


    void FixedUpdate()
    {
        this.transform.position += new Vector3(speed,0,0);
        if(transform.position.x > ending_loc.x)
        {
            transform.position = starting_loc;
        }
    }
   
}
