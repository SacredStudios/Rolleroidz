using UnityEngine;
using Photon.Pun;

public class AI_Handler : MonoBehaviour
{
    public byte team;
    public int score;
    [SerializeField] PhotonView pv;

    public void AddScore(int value)
    {
        pv.RPC("SyncScore", RpcTarget.All, score + value) ;
    }
    public void DivideScore()
    {
        pv.RPC("SyncScore", RpcTarget.All, score / 2);
    }
    [PunRPC] public void SyncScore(int new_score)
    {
        Debug.Log(new_score + " is the new score");
        score = new_score;
    }
}
