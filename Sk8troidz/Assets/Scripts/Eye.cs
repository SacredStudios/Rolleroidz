using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Eye : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject blink;
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float maxMovementRadius = 0.5f;
    [SerializeField] bool invertMovement = false;
    [SerializeField] PhotonView pv;

    private Vector3 initialPupilPosition;

    void Start()
    {
        StartCoroutine(Blink());
        if (pv.IsMine)
        {
            // Ensure that the camera reference is set.
            if (mainCamera == null)
            {
                Debug.LogError("Main camera reference is not set in the Eye script!");
                enabled = false;
                return;
            }
        }


        // Store the initial position of the pupil relative to the eye.
  
        initialPupilPosition = transform.InverseTransformPoint(transform.GetChild(0).position);
    }

    IEnumerator Blink()
    {
        blink.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        blink.SetActive(false);
        yield return new WaitForSeconds(Random.Range(10, 20));
        yield return Blink();
    }

    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            // Get the mouse position in screen coordinates.
            Vector3 mousePosition = Input.mousePosition;

            // Convert the mouse position to a world position relative to the camera.
            Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z));

            // Calculate the desired local position of the pupil.
            Vector3 desiredPupilPosition = transform.InverseTransformPoint(worldMousePosition);

            // Ensure the pupil doesn't move on the Y-axis (optional).
            desiredPupilPosition.y = initialPupilPosition.y;

            if (invertMovement)
            {
                // Invert the movement direction
                Vector3 invertedOffset = (desiredPupilPosition - initialPupilPosition) * -1;
                desiredPupilPosition = initialPupilPosition + invertedOffset;
            }

            // Check if the desired position exceeds the maximum radius.
            Vector3 offset = desiredPupilPosition - initialPupilPosition;
            if (offset.magnitude > maxMovementRadius)
            {
                desiredPupilPosition = initialPupilPosition + offset.normalized * maxMovementRadius;
            }

            // Move the pupil towards the desired position smoothly.
            Vector3 newPupilPosition = Vector3.Lerp(initialPupilPosition, desiredPupilPosition, Time.deltaTime * sensitivity);

            // Apply the new local position to the pupil.
            pv.RPC("MoveEye", RpcTarget.Others, newPupilPosition);
            transform.GetChild(0).localPosition = newPupilPosition;
        }
    }
    [PunRPC]
    void MoveEye(Vector3 newPosition)
    {
        transform.GetChild(0).localPosition = newPosition;
    }
}
