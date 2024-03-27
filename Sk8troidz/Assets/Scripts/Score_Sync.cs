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
    [SerializeField] Text score_mine;
    [SerializeField] bool game_ongoing;
    [SerializeField] PhotonView pv;
    [SerializeField] GameObject rolling_meter;
    

    private void Update()
    {
        if (!score.text.Equals("" + pv.Owner.GetScore()))
        {
            score.text = "" + pv.Owner.GetScore();
            rolling_meter.GetComponent<Rolling_Meter>().ScrollToNumber(pv.Owner.GetScore());
        }
    }
   

         
}
