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
    [SerializeField] GameObject weapon_selector;
    public Weapon my_weapon;
    GameObject weapon_list;



    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        weapon_list = GameObject.Find("WeaponList"); //I know gameobject.find is bad. Do you have any better ideas?

    }
    private void Start()
    {
        my_weapon = Weapon_Selector.curr_weapon;
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
        Debug.Log(new_player.name);
        if (new_player.GetComponentInChildren<PlayerMovement>() != null)
        {
            new_player.GetComponentInChildren<PlayerMovement>().enabled = false;
            new_player.GetComponentInChildren<Weapon_Handler>().enabled = false;
        }
        else
        {
            Destroy(new_player);
        }
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
        pv.RPC("SpawnPlayer", RpcTarget.All, weapon_list.GetComponent<Weapon_List>().weapon.name);
    }
    [PunRPC]
    public void SpawnPlayer(string name)
    {
        Debug.Log("test");
        position = transform.position;
        lobby_cam.SetActive(false);
        lobby.SetActive(false);
        /*new_player = PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
        new_player.GetComponent<Respawn>().respawn_points = respawn_points.GetComponent<RespawnPoints>().respawn_points;
        foreach (Weapon w in weapon_list.GetComponent<Weapon_List>().all_weapon_list)
            if (w.name.Equals(name))
            {
                new_player.GetComponentInChildren<Weapon_Handler>().weapon = w;
                Debug.Log("foundweapon");
            }
        
    }*/

    }
}


