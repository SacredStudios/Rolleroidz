using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BlackHole : MonoBehaviourPun
{
    [SerializeField] float pullRadius = 10f;
    [SerializeField] float basePullForce = 50f;

    Rigidbody localBody;     // your own avatar’s body
    bool affectsMe;     // set once by the RPC

    /* ───────────────── Spawn helper ─────────────────
       Call this from the player that creates the hole. */
    public static void Spawn(Vector3 pos, Quaternion rot, GameObject player)
    {
        int team = player.GetComponent<Team_Handler>().GetTeam();

        GameObject hole = PhotonNetwork.Instantiate("BlackHole", pos, rot);
        hole.GetComponent<PhotonView>()
            .RPC(nameof(Init), RpcTarget.AllBuffered, team);
    }

    /* ───────────────── Init (runs once per client) ───────────────── */
    [PunRPC]
    void Init(int spawnerTeam)
    {
        // find *our* avatar (owns a PhotonView with IsMine==true)
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
            var pv = go.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                localBody = go.GetComponent<Rigidbody>();
                int myTeam = go.GetComponent<Team_Handler>().GetTeam();
                affectsMe = myTeam != spawnerTeam;        // enemies only
                break;
            }
        }
    }

    /* ───────────────── Physics ───────────────── */
    void FixedUpdate()
    {
        if (!affectsMe || localBody == null) return;

        float dist = Vector3.Distance(transform.position, localBody.position);
        if (dist > pullRadius || dist < 0.01f) return;

        bool isAI = localBody.CompareTag("AI_Player");
        float strength = isAI
                         ? basePullForce / (dist * dist)
                         : 20f * basePullForce / (dist * dist);

        Vector3 dir = (transform.position - localBody.position).normalized;
        localBody.AddForce(dir * strength, ForceMode.Acceleration);
    }
}
