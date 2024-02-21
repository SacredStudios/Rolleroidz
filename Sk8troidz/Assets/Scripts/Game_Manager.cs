using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;
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

    [SerializeField] GameObject gameover_screen;
    [SerializeField] Text gameover_text;
    [SerializeField] GameObject countdown;
    public Weapon my_weapon;
    GameObject weapon_list;



    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        weapon_list = GameObject.Find("WeaponList"); //I know gameobject.find is bad. Do you have any better ideas?
        countdown.SetActive(false);
        Debug.Log(PlayerPrefs.GetFloat("sensitivity"));

    }
    private void Start()
    {
        my_weapon = weapon_list.GetComponent<Weapon_List>().curr_weapon;
        pv.Owner.SetScore(0);
        PhotonNetwork.LocalPlayer.JoinTeam((byte)Random.Range(1, 3));
        
        StartCoroutine(SwitchTeam(PhotonNetwork.LocalPlayer));
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckForPlayers());
        }

    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
        }
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
    IEnumerator LeaveTeam()
    {
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPhotonTeam() == null);
        PhotonNetwork.LeaveRoom();

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
        pv.RPC("Start_Countdown", RpcTarget.All);
        Invoke("SpawnPlayers", 5f);
        
    }
    [PunRPC] public void Start_Countdown()
    {
        countdown.SetActive(true);
        StartCoroutine(Countdown(5));
    }
    IEnumerator Countdown(int n)
    {
        while (n > 0)
        {
            countdown.GetComponent<Text>().text = n.ToString();
            yield return new WaitForSeconds(1);
            n--;
        }

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
        Cursor.lockState = CursorLockMode.None;
        if (new_player.GetComponentInChildren<PlayerMovement>() != null)
        {
            new_player.GetComponentInChildren<PlayerMovement>().enabled = false;
            new_player.GetComponentInChildren<Weapon_Handler>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // new_player.
        }
        else
        {
            new_player.GetComponentInChildren<Camera>().transform.parent = null;
            new_player.SetActive(false);
        }
        Debug.Log(winningteam + " + " +PhotonNetwork.LocalPlayer.GetPhotonTeam().Code);
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == winningteam)
        {
            Invoke("WinScreen", 1f);
        }
        else
        {
            Invoke("LoseScreen", 1f);
        }
        
    }
    void WinScreen()
    {
        gameover_screen.SetActive(true);
        gameover_text.text =" You Win!";
        Debug.Log("You Win");
    }
    void LoseScreen()
    {
        gameover_screen.SetActive(true);
        gameover_text.text = " You Lose!";
        Debug.Log("You Lose");
    }
    public void BackToStart()
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        StartCoroutine(LeaveTeam());
        
        
    }
    public override void OnLeftRoom()
    {
        

        Debug.Log(PhotonNetwork.LocalPlayer.GetPhotonTeam());
        
        PhotonNetwork.LoadLevel("StartingScene");
        
        
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
    [PunRPC]
    public void SpawnPlayer()
    {
       
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        print(my_weapon);
        position = transform.position;
        lobby_cam.SetActive(false);
        lobby.SetActive(false);
        if (new_player == null)
        {
            new_player = PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
            List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points; //respawn locations
            new_player.GetComponent<Respawn>().respawn_points = points;
            new_player.GetComponentInChildren<Weapon_Handler>().weapon = my_weapon;
            Debug.Log(points.Count);
            new_player.transform.position = points[Random.Range(0, points.Count)];
            Debug.Log(new_player.transform.position);
        }




    }
   

    }



