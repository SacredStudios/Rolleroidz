using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Team_Handler : MonoBehaviour
{
    public byte ai_team; //set this only with ai players

    public int GetTeam()
    {
        if (this.gameObject.tag == "Player")
        {
            return GetComponent<PhotonView>().Owner.GetPhotonTeam().Code;
        }
        if (this.gameObject.tag == "AI_Player")
        {
            return ai_team;
        }
        else
        {
            return 0;
        }
    }

    public int GetScore()
    {
        if (this.gameObject.tag == "Player")
        {
            return GetComponent<PhotonView>().Owner.GetScore();
        }
        if (this.gameObject.tag == "AI_Player")
        {
            return GetComponent<AI_Handler>().score;
        }
        else
        {
            return 0;
        }
    }
   
}
