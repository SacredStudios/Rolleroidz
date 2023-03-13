using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
public class PointManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text score;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        score.text = "" + PhotonNetwork.LocalPlayer.GetScore();
    }
}
