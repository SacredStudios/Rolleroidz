using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class Ragdoll : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] public List<Collider> colliders;
    PlayerMovement pm;
    public static bool is_Ragdoll;
    [SerializeField] Rigidbody rb;
    [SerializeField] PhotonView pv;
    [SerializeField] float timeout;
    [SerializeField] CinemachineVirtualCamera camera;
    [SerializeField] GameObject ragdoll_follow;
    [SerializeField] GameObject camera_follow;
    [SerializeField] AudioSource trick_fail;
    [SerializeField] AudioSource crowd_boo;
    [SerializeField] GameObject crosshair;
    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    public void ActivateRagdolls()
    {
        crowd_boo.Play();
        if(pv.IsMine)
        {
            Crosshair_Inactive();
            int id = gameObject.GetComponent<PhotonView>().ViewID;
            pv.RPC("SyncRagdoll", RpcTarget.All, id);
        }
        pm.enabled = false;
        camera.GetComponent<CinemachineVirtualCamera>().Follow = ragdoll_follow.transform;
        GetComponent<Weapon_Handler>().weapon = null;
        is_Ragdoll = true;
        rb.linearVelocity = new Vector3(0, 0, 0);
        Invoke("DeactivateRagdolls", timeout);
       
    }
    [PunRPC] void SyncRagdoll(int id)
    {
       trick_fail.Play();
       GameObject player = PhotonView.Find(id).gameObject;
       player.GetComponent<Animator>().enabled = false;
        foreach (Collider collider in player.GetComponent<Ragdoll>().colliders)
        {
            collider.enabled = true;
            collider.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
        }

    }
    [PunRPC]
    void DeSyncRagdoll(int id) //for whenever ragdoll is disabled. I know, poor choice of words.
    {
        GameObject player = PhotonView.Find(id).gameObject;
        player.GetComponent<Animator>().enabled = true;
        foreach (Collider collider in player.GetComponent<Ragdoll>().colliders)
        {
            collider.enabled = false;
        }

    }
    public void Crosshair_Inactive()
    {
        crosshair.SetActive(false);
    }
    public void DeactivateRagdolls()
    {
        PlayerMovement.currentPrompt = PlayerMovement.TrickPrompt.None;
        crosshair.SetActive(true);
        GetComponent<Animator>().enabled = true;
        pm.enabled = true;
        camera.GetComponent<CinemachineVirtualCamera>().Follow = camera_follow.transform;
        if (pm.canJump)
        {
            pm.Jump();
        }
        is_Ragdoll = false;
        int id = gameObject.GetComponent<PhotonView>().ViewID;
        pv.RPC("DeSyncRagdoll", RpcTarget.All, id);       
    }
}
