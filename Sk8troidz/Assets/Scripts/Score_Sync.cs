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
    void Start()
    {
        StartCoroutine(SyncScore());
    }

    IEnumerator SyncScore()
    {
        while (game_ongoing)
        {
            score.text = "" + pv.Owner.GetScore();

            yield return new WaitForSeconds(1f);
        }
    }
}
