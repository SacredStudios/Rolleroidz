using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float rotationSpeed = 100.0f;
    [SerializeField] Camera mainCamera;
    bool canSpin;

    private void Start()
    {
        canSpin = false;
    }
    public void Spin_Enabled()
    {
        canSpin = true;
    }
    void Update()
    {
        if (Input.GetMouseButton(0) && canSpin)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // Check if the raycast hit this GameObject
                {
                    float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                    transform.Rotate(Vector3.up, mouseX);
                }
            }
        }
    }
}

