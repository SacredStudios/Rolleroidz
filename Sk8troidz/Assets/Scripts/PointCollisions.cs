using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCollisions : MonoBehaviour
{
    [SerializeField] PointTally pt;
  

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Point")
        {
            Destroy(collider.gameObject);
            pt.ChangePoints(1);
            Debug.Log(pt.points);
        }
    }
}
