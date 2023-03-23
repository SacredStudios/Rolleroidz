using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

//MERGE THIS WITH GAMEMANAGER
public class PointManager : MonoBehaviourPunCallbacks
{
    public static bool game_ongoing;
    int score1 = 0;
    int score2 = 0;
    void OnEnable()
    {
        game_ongoing = true;
        StartCoroutine(PointChecker());
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        StartCoroutine(PointChecker());
    }
        IEnumerator PointChecker()
    {
        int oldscore1 = score1;
        int oldscore2 = score2;
        
        while (game_ongoing)
        {
            
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
                    Debug.Log("TEST ETSTA");

                }
            }
            yield return new WaitUntil(() => score1 != oldscore1 || score2 != oldscore2);
            Debug.Log(score1 + "+" + score2);
            
            
        }
    }
}
