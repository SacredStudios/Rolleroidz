using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class TeamManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text Team1List;
    [SerializeField] Text Team2List;
    [SerializeField] PhotonTeamsManager tm;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);   
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();


        PhotonNetwork.LocalPlayer.JoinTeam(1);
        int curr = tm.GetTeamMembersCount(1);
        Player[] players = new Player[10];

        new WaitForSeconds(5f);

        Debug.Log(tm.GetTeamMembersCount(1));
        tm.TryGetTeamMembers(1, out players);
        /*Team1List.text = "";
        for (int i = 0; i < players.Length; i++)
        {

            Team1List.text += players[i].NickName + "\n";
        }
        */
    }
}
