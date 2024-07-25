using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class AI_Handler : MonoBehaviour
{
    public byte team;
    public int score;
    [SerializeField] Text text;
    [SerializeField] PhotonView pv;

    public void AddScore(int value)
    {
        pv.RPC("SyncScore", RpcTarget.All, score + value);
    }
    public void DivideScore()
    {
        pv.RPC("SyncScore", RpcTarget.All, score / 2);
    }
    [PunRPC] public void SyncScore(int new_score)
    {
        text.text = "" + new_score;
        Debug.Log(new_score + " is the new score");
        if (this.gameObject.tag == "AI_Player")
        {
            score = new_score;
        }
    }
}
