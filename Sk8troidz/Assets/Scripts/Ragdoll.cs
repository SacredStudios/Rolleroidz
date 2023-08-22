using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ragdoll : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] public List<Collider> colliders;
    PlayerMovement pm;
    public static bool is_Ragdoll;
    [SerializeField] Rigidbody rb;
    [SerializeField] PhotonView pv;
    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    public void ActivateRagdolls ()
    {
        if(pv.IsMine)
        {
            int id = gameObject.GetComponent<PhotonView>().ViewID;
            pv.RPC("SyncRagdoll", RpcTarget.All, id);
        }
        
        pm.enabled = false;
        GetComponent<Weapon_Handler>().weapon = null;
        is_Ragdoll = true;
        rb.velocity = new Vector3(0, 0, 0);
       // Invoke("DeactivateRagdolls", 3f);
       
    }
    [PunRPC] void SyncRagdoll(int id)
    {
       GameObject player = PhotonView.Find(id).gameObject;
       player.GetComponent<Animator>().enabled = false;
        foreach (Collider collider in player.GetComponent<Ragdoll>().colliders)
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
