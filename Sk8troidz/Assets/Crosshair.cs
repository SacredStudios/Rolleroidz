using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour
{
    public Image crosshair;
    public CinemachineVirtualCamera cinemachineCamera; // Reference to the Cinemachine Virtual Camera
    public GameObject weapon_loc;
    public float maxDistance = 100f;
    public Camera camera;

    public float verticalAdjustmentFactor = 50f;

    void Update()
    {
        Ray ray = new Ray(weapon_loc.transform.position, weapon_loc.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            MoveCrosshairBasedOnPitch();
        }
    }

    void MoveCrosshairBasedOnPitch()
    {
        Camera mainCamera = camera;

        // Get the pitch from the camera's rotation
        float pitch = mainCamera.transform.eulerAngles.x;

        // Normalize pitch to the range of -90 to 90
        if (pitch > 180)
            pitch -= 360;

        pitch /= 180f; // Normalize pitch to -0.5 to 0.5 range

        float adjustment = -pitch * verticalAdjustmentFactor;

        // Just debug the adjustment to see how much it's influencing
        Debug.Log("Adjustment: " + adjustment);

        // Adjust the y position of the reticle based on the pitch
        crosshair.transform.position = new Vector3(crosshair.transform.position.x,
                                                   Screen.height / 2 + adjustment,
                                                   crosshair.transform.position.z);
    }



}



