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
    [SerializeField] GameObject Player;
    public float verticalAdjustmentFactor = 50f;  // Factor to adjust crosshair based on pitch
    public float verticalOffset = 50f;            // Offset to move the crosshair up from the center


    private void Start()
    {
        Debug.Log(Player.GetComponent<Weapon_Handler>().weapon.range);
        maxDistance = (Player.GetComponent<Weapon_Handler>().weapon.range);
        verticalOffset = maxDistance / 5f;
        verticalAdjustmentFactor = verticalOffset * 1.8f;
    }
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

        float adjustment = pitch * verticalAdjustmentFactor;

        // Adjust the y position of the reticle based on the pitch and the static offset
        crosshair.transform.position = new Vector3(crosshair.transform.position.x,
                                                   Screen.height / 2 + verticalOffset + adjustment,
                                                   crosshair.transform.position.z);
    }
}
