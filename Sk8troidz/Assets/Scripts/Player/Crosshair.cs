using UnityEngine;
using Photon.Pun;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private PhotonView pv;          // PhotonView for multiplayer ownership
    [SerializeField] private Camera cam;            // Main camera
    [SerializeField] private Transform gunBarrel;   // Gun barrel position
    [SerializeField] private float maxDistance = 100f; // Max raycast range

    private RectTransform crosshairRectTransform;   // Crosshair's RectTransform
    private Canvas parentCanvas;                    // Parent Canvas

    private void Start()
    {
        // Get components
        crosshairRectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        // Disable for non-local players
        if (!pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        // Ensure pivot and anchors are centered
        crosshairRectTransform.pivot = new Vector2(0.5f, 0.5f);
        crosshairRectTransform.anchoredPosition = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!pv.IsMine) return;

        // Perform raycast in the gunBarrel's forward direction
        Vector3 rayOrigin = gunBarrel.position;
        Vector3 rayDirection = gunBarrel.forward; // Use gunBarrel.forward for raycast direction

        Ray ray = new Ray(rayOrigin, rayDirection);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow); // Visualize ray

        Vector3 targetPoint;

        // Check for hits
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            targetPoint = hit.point; // Ray hits a target
        }
        else
        {
            targetPoint = rayOrigin + rayDirection * maxDistance; // Default to max range
        }

        // Convert world position to screen space
        Vector3 screenPosition = cam.WorldToScreenPoint(targetPoint);

        if (screenPosition.z > 0) // Ensure the target is in front of the camera
        {
            Vector2 localPoint;

            // Convert screen position to local position within the UI Canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                null, // Camera is null for Screen Space - Overlay
                out localPoint))
            {
                // Update the crosshair position
                crosshairRectTransform.anchoredPosition = localPoint;
            }
        }
        else
        {
            // Reset to screen center if position is invalid
            crosshairRectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
