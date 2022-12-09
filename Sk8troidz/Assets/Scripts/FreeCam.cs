using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject player;
    [SerializeField] Vector3 offset;
    [SerializeField] float sensitivity = 0.05f;
    [SerializeField] Vector2 yBoundaries;
    [SerializeField] Vector2 xBoundaries;
    public float testRot;
    // Update is called once per frame
    [SerializeField] Camera cam;
    void Update()
    {
        Vector3 vp = cam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane*0.05f));

        vp.x -= 0.5f;
        vp.y -= 0.5f;
        vp.x *= sensitivity;
        vp.y *= sensitivity;
        vp.x += 0.5f;
        vp.y += 0.5f;

        Vector3 sp = cam.ViewportToScreenPoint(vp);
        Vector3 v = cam.ScreenToWorldPoint(sp);
        transform.LookAt(v, Vector3.up);
        Debug.Log(transform.localEulerAngles);

        if (Mathf.Abs((transform.eulerAngles.x)) > 80) transform.localEulerAngles = new Vector3(80, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
    private void Start()
    {

       

    }
}
