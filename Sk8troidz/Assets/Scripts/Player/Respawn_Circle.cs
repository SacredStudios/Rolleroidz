using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required for checking UI elements

public class Respawn_Circle : MonoBehaviour
{
    public Camera RespawnCam; // Assign your main camera here
    [SerializeField] GameObject player;
    [SerializeField] GameObject respawn_btn;

    private void Start()
    {
        RespawnCam = GameObject.Find("RespawnCam").GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() )
        {
            Ray ray = RespawnCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                transform.position = hit.point - new Vector3(0, -10, 0);
            }
        }
        if (this.gameObject.transform.position.y > 50)
        {
            transform.position = new Vector3(transform.position.x, 50, transform.position.z);
        }
    }
}
