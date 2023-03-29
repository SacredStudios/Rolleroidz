using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Game_Manager : MonoBehaviourPunCallbacks
{
    public GameObject player_prefab;
    Vector3 position;
    [SerializeField] GameObject respawn_points;
    
    [SerializeField] int min_room_size;
    [SerializeField] GameObject lobby;
  
    [SerializeField] GameObject lobby_cam;
    [SerializeField] PhotonTeamsManager tm;
    [SerializeField] Text Team1List;
    [SerializeField] Text Team2List;
    [SerializeField] PhotonView pv;
    [SerializeField] int temp1 = 9999; //check to see if teamsize has been received from MasterClient
    [SerializeField] int temp2 = 9999;
    bool game_ongoing = false;
    [SerializeField] int team1count;
    [SerializeField] int team2count;
    [SerializeField] int win_score = 15;
    [SerializeField] GameObject new_player;




    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }
    private void Start()
    {
        PhotonNetwork.LocalPlayer.JoinTeam((byte)Random.Range(1, 3));
        StartCoroutine(SwitchTeam(PhotonNetwork.LocalPlayer));
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckForPlayers());
        }

    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PropChange();
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PropChange();
    }
    IEnumerator SwitchTeam(Player player)
    { 
        foreach (Player player_temp in PhotonNetwork.PlayerList)
        {
            yield return new WaitUntil(() => player_temp.GetPhotonTeam() != null);
        }
        yield return new WaitUntil(() => temp1 != 9999 && temp2 != 9999);
        if (temp1 < temp2) //get team count from master client
        {
            player.SwitchTeam(1);
        }
        else if (temp1 > temp2)
        {
            player.SwitchTeam(2);
        }
        
        
    }
    IEnumerator CheckForPlayers() //Checks if teams have been assigned correctly before starting game
    {
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount >= min_room_size);
        foreach (Player player_temp in PhotonNetwork.PlayerList)
        {
            yield return new WaitUntil(() => player_temp.GetPhotonTeam() != null);
        }
        Debug.Log("StartingGame");
        game_ongoing = true;
        Invoke("SpawnPlayers", 5f);
        
    }
    [PunRPC] public void GetTeams(int x, int y)
    {
        temp1 = x;
        temp2 = y;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
       if(PhotonNetwork.IsMasterClient)
        {
                pv.RPC("GetTeams", RpcTarget.All, tm.GetTeamMembersCount(1), tm.GetTeamMembersCount(2));
                team1count = 0;
                team2count = 0;
                foreach (Player player in PhotonNetwork.PlayerList)
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
                if (team1count >= win_score)
                {
                  pv.RPC("GameOver", RpcTarget.All, 1);
                }
                else if (team2count >= win_score)
                {
                  pv.RPC("GameOver", RpcTarget.All, 2);
                }
        }
        PropChange();
        
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    
    [PunRPC]
    public void GameOver(int winningteam)
    {
        new_player.GetComponent<PlayerMovement>().enabled = false;
        new_player.GetComponent<Weapon_Handler>().enabled = false;
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == winningteam)
        {
            Invoke("WinScreen", 5f);
        }
        else
        {
            Invoke("LoseScreen", 5f);
        }
        
    }
    void WinScreen()
    {
        Debug.Log("You Win");
    }
    void LoseScreen()
    {
        Debug.Log("You Lose");
    }
    void PropChange()
    {
        Team1List.text = "";
        Team2List.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPhotonTeam().Code == 1)
            {
                Team1List.text += player.NickName + "\n";
            }
            else if(player.GetPhotonTeam().Code == 2)
            {
                Team2List.text += player.NickName + "\n";
            }
            else
            {
                StartCoroutine(SwitchTeam(player));
            }
        }
    }

    public void SpawnPlayers()
    {    
        pv.RPC("SpawnPlayer", RpcTarget.All);
    }
    [PunRPC] public void SpawnPlayer()
    {
        position = transform.position;
        new_player = PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
        new_player.GetComponent<Respawn>().respawn_points = respawn_points.GetComponent<RespawnPoints>().respawn_points;
        lobby_cam.SetActive(false);
        lobby.SetActive(false);
    }
  
    
}


