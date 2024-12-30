using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
public class Scores : MonoBehaviourPunCallbacks
{
    int team1count;
    int team2count;
    GameObject[] ai_players;

    [SerializeField] Text red_team;
    [SerializeField] Text pink_team;

    private void Start()
    {
        ai_players = GameObject.FindGameObjectsWithTag("AI_Player");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            team1count = 0;
            team2count = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetPhotonTeam() != null)
                {
                    if (player.GetPhotonTeam().Code == 1)
                    {
                        team1count += player.GetScore();
                    }
                    else
                    {
                        team2count += player.GetScore();
                    }
                }
            }
            if (ai_players != null)
            {
                foreach (GameObject player in ai_players)
                {
                    if (player.GetComponent<Team_Handler>().GetTeam() == 1)
                    {

                        team1count += player.GetComponent<AI_Handler>().score;
                    }
                    if (player.GetComponent<Team_Handler>().GetTeam() == 2)
                    {

                        team2count += player.GetComponent<AI_Handler>().score;
                    }

                }
            }
            red_team.text = "" + team1count;
            pink_team.text = "" + team2count;
        }
    }
}