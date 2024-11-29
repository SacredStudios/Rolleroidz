using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;

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
        //This is just so OnPlayerPropertiesUpdate() is called even for AI players
        Hashtable nullFunctionProperty = new Hashtable
        {
            { "dummyKey", null }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(nullFunctionProperty);
        text.text = "" + new_score;
        if (this.gameObject.tag == "AI_Player")
        {
            score = new_score;
        }
    }
}
