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
    [SerializeField] Text list;
    [SerializeField] PhotonView pv;
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
    List<GameObject> ai_players;

    int count = 0;
    [SerializeField] GameObject AIPlayer;
    [SerializeField] GameObject AIPlayer2;
    [SerializeField] GameObject start_early;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        weapon_list = GameObject.Find("WeaponList"); //I know gameobject.find is bad. Do you have any better ideas?
        countdown.SetActive(false);
        if (!PhotonNetwork.IsMasterClient)
        {
            start_early.SetActive(false);
        }

    }
    private void Start()
    {
        my_weapon = weapon_list.GetComponent<Weapon_List>().curr_weapon;
        pv.Owner.SetScore(0);
        Invoke("CheckForPlayersOnTime", 30f);
        //PhotonNetwork.LocalPlayer.JoinTeam((byte)Random.Range(1, 3));
    }
    [SerializeField] Chat_Manager chatManager;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            chatManager.SendStatMessage(newPlayer.NickName + " entered the room.");
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
        }
        base.OnPlayerEnteredRoom(newPlayer);
        PropChange();
        if (PhotonNetwork.CurrentRoom.PlayerCount >= min_room_size && PhotonNetwork.IsMasterClient) //why do we need this ->newPlayer == PhotonNetwork.LocalPlayer &&
        {
            StartCoroutine(CheckForPlayers());
        }
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PropChange();
    }
    IEnumerator SwitchTeams()
    {
        
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //sometimes the player is still on the same team as last game, so this is a check to
            //set it to null
            player.LeaveCurrentTeam();
            yield return new WaitUntil(() => player.GetPhotonTeam() == null);
            if (++count % 2 == 0)
            {
                player.JoinTeam(1);
            }
            else
            {
                player.JoinTeam(2);
            }
            yield return new WaitUntil(() => player.GetPhotonTeam() != null);
        }

        yield return new WaitForSeconds(5);
        pv.RPC("SpawnPlayer", RpcTarget.All);


    }
   
    IEnumerator LeaveTeam()
    {
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPhotonTeam() == null);
        PhotonNetwork.LeaveRoom();

    }
    IEnumerator CheckForPlayers() //Checks if teams have been assigned correctly before starting game
    {
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount >= min_room_size || 1==1); //delete this part in official builds
        game_ongoing = true;
        pv.RPC("Start_Countdown", RpcTarget.All);
        SpawnAIPlayers();
        yield return new WaitForSeconds(5); //test to see if this can be a shorter time
        SpawnPlayers();
        
    }

    //Checks for players after 30 second timer is up
    public void CheckForPlayersOnTime()
    {
        if (start_early.activeSelf)
        {
            start_early.SetActive(false);
            StartCoroutine(CheckForPlayers());
        }
    }
    //Checks for players after button is pressed
    public void CheckForPlayersDebug()
    {
        start_early.SetActive(false);
        StartCoroutine(CheckForPlayers());
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
    private void Update()
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
       if(PhotonNetwork.IsMasterClient)
        {
                team1count = 0;
                team2count = 0;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if(player.GetPhotonTeam() != null) {
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
                        Debug.Log(team1count + "+" + team2count);
                    }
                }

                if (team1count >= win_score || team2count >= win_score)
                    {
                        Invoke("DoubleCheck", 3f);
                    }
               
        }

        PropChange();
        
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
    void DoubleCheck()
    {
        //pv.RPC("GetTeams", RpcTarget.All, tm.GetTeamMembersCount(1), tm.GetTeamMembersCount(2));
        //TODO: Add AI_players
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
        if (ai_players != null)
        {
            foreach (GameObject player in ai_players)
            {
                if (player.GetComponent<Team_Handler>().GetTeam() == 1)
                {
                    team1count += player.GetComponentInChildren<AI_Handler>().score;
                }
                else
                {
                    team2count += player.GetComponentInChildren<AI_Handler>().score;
                }
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

    
    [PunRPC]
    public void GameOver(int winningteam)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
            Cursor.lockState = CursorLockMode.None;
        if (new_player.GetComponentInChildren<PlayerMovement>() != null)
        {
            new_player.GetComponentInChildren<PlayerMovement>().enabled = false;
            new_player.GetComponentInChildren<Weapon_Handler>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        lobby_cam.SetActive(true);
        gameover_screen.SetActive(true);
        gameover_text.text =" YOU WIN";
        Debug.Log("You Win");
    }
    void LoseScreen()
    {
        lobby_cam.SetActive(true);
        gameover_screen.SetActive(true);
        gameover_text.text = " YOU LOSE";
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
        list.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            list.text += player.NickName + "\n";           
        }
    }

    public void SpawnPlayers()
    {
        StartCoroutine(SwitchTeams());
        
    }
    void SpawnAIPlayers()
    {
        ai_players = new List<GameObject>();
        Debug.Log(team1count + "+" + team2count);
        while (count < min_room_size- PhotonNetwork.PlayerList.Length)
        {
            count++;
            //A lot of redundant code here
            if (count % 2 == 0)
            {
                GameObject ai_player = PhotonNetwork.Instantiate(AIPlayer2.name, position, Quaternion.identity, 0);
                ai_player.GetComponentInChildren<AI_Weapon_Handler>().chat_manager = chatManager.GetComponent<Chat_Manager>();
                List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points; //respawn locations
                ai_player.GetComponent<Respawn>().respawn_points = points;
                ai_player.transform.position = points[Random.Range(0, points.Count)];
                ai_players.Add(ai_player.transform.GetChild(0).gameObject);
            }
            else
            {
                GameObject ai_player = PhotonNetwork.Instantiate(AIPlayer.name, position, Quaternion.identity, 0);
                ai_player.GetComponentInChildren<AI_Weapon_Handler>().chat_manager = chatManager.GetComponent<Chat_Manager>();
                List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points; //respawn locations
                ai_player.GetComponent<Respawn>().respawn_points = points;
                ai_player.transform.position = points[Random.Range(0, points.Count)];
                ai_players.Add(ai_player.transform.GetChild(0).gameObject);
            }
        }
        //Debug.Log(ai_players[0] + "+" + ai_players[1] + "+" + ai_players[2]);
    }
    [PunRPC]
    public void SpawnPlayer()
    {
       
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        position = transform.position;
        lobby_cam.SetActive(false);
        lobby.SetActive(false);
        if (new_player == null)
        {
            new_player = PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
            List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points; //respawn locations
            new_player.GetComponent<Respawn>().respawn_points = points;
            new_player.GetComponentInChildren<Weapon_Handler>().weapon = my_weapon;
            new_player.GetComponentInChildren<Weapon_Handler>().weapon.chat_manager = chatManager.GetComponent<Chat_Manager>();
            new_player.transform.position = points[Random.Range(0, points.Count)];
        }
        Debug.Log(new_player.GetComponentInChildren<Camera>());
        AI_LookAt.cam = new_player.GetComponentInChildren<Camera>();
        AI_LookAt.pv = new_player.GetComponent<PhotonView>();




    }
   

    }



