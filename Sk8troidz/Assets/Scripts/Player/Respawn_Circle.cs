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
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) // Check if left mouse button is held down and not over a UI element
        {
            Ray ray = RespawnCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Update the position of the circle while the mouse button is held
                transform.position = hit.point - new Vector3(0, -10, 0);
            }
        }
    }
}
