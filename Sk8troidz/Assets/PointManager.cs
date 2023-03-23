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
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log(p.GetScore());
            }
            yield return new WaitForSeconds(1);
        }
    }
}
