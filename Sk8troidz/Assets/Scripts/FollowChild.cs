using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChild : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject child;
    [SerializeField] Vector3 offset;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = child.transform.position +offset;
    }
}
