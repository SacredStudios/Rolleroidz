using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Crosshair : MonoBehaviour
{
    [SerializeField] PhotonView pv;
    [SerializeField] Camera cam;
    [SerializeField] GameObject player;
    [SerializeField] float multiplier;
    [SerializeField] Vector3 rotationOffset; // Rotation offset
    private void Start()
    {
        if (!pv.IsMine)
        {
            this.gameObject.SetActive(false);
        }

        float maxDistance = player.GetComponent<Weapon_Handler>().weapon.range;
        this.transform.position = this.transform.position+ new Vector3(0, maxDistance*multiplier, 0);
        

    }
    void LateUpdate()
    {
        // Apply the rotation offset to the object's rotation
        Quaternion rotationWithOffset = cam.transform.rotation * Quaternion.Euler(rotationOffset);

        // Update the object's rotation
        transform.LookAt(transform.position + rotationWithOffset * Vector3.forward, rotationWithOffset * Vector3.up);
    }

}