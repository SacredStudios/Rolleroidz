using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class PointManager : MonoBehaviourPunCallbacks
{
    public static bool game_ongoing;
    void OnEnable()
    {
        game_ongoing = true;
        StartCoroutine(PointChecker());
    }

    IEnumerator PointChecker()
    {
        
        while (game_ongoing)
        {
            int score1 = 0;
            int score2 = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if(p.GetPhotonTeam().Code == 1)
                    {
                        score1 += p.GetScore();
                    }
                    else
                    {
                        score2 += p.GetScore();
                    }
                    Debug.Log(score1 + "+" + score2);
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}
