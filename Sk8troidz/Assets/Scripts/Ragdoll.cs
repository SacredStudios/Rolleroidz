using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Collider> colliders;
    PlayerMovement pm;
    public static bool is_Ragdoll;
    [SerializeField] Rigidbody rb;
    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    public void ActivateRagdolls ()
    {
        GetComponent<Animator>().enabled = false;
        pm.enabled = false;
        GetComponent<Weapon_Handler>().weapon = null;
        is_Ragdoll = true;
        rb.velocity = new Vector3(0, 0, 0);
        Invoke("DeactivateRagdolls", 3f);
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
            collider.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
    public void DeactivateRagdolls()
    {
        GetComponent<Animator>().enabled = true;
        pm.enabled = true;
        if (pm.canJump)
        {
            pm.rb.AddForce(Vector3.up * pm.jumpStrength);
        }
        is_Ragdoll = false;
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
