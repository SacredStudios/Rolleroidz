using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class Score_Sync : MonoBehaviour
{
    [SerializeField] Text score;
    [SerializeField] bool game_ongoing;
    [SerializeField] PhotonView pv;
    

    private void Update()
    {
        if (!score.text.Equals("" + PhotonNetwork.LocalPlayer.GetScore()))
        {
           // pv.Owner.AddScore(0);
            score.text = "" + PhotonNetwork.LocalPlayer.GetScore();
        }
    }
   

         
}
