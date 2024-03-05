using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChild : MonoBehaviour
{
    [SerializeField] GameObject child;
    [SerializeField] Vector3 offset;
    
    void Update()
    {
      //  this.gameObject.transform.position = child.transform.position +offset;
    }
}
