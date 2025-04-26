using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
    [SerializeField] GameObject end_player;
    [SerializeField] GameObject weapon_selector;

    [SerializeField] GameObject gameover_screen;
    [SerializeField] Text gameover_text;
    [SerializeField] GameObject countdown;
    public Weapon my_weapon;
    GameObject weapon_list;
    List<GameObject> ai_players;
    List<GameObject> players;

    int count = 0;
    [SerializeField] GameObject AIPlayer;
    [SerializeField] GameObject AIPlayer2;
    [SerializeField] GameObject start_early;
    [SerializeField] GameObject end_screen;
    [SerializeField] GameObject end_cam;
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
        Respawn.isOver = false;
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
                      //  Debug.Log(team1count + "+" + team2count);
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
    int whoWon = 1;
    bool win = false; //checks if local player won or not
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
            whoWon = 2;
            pv.RPC("GameOver", RpcTarget.All, 2);
        }
    }

    
    [PunRPC]
    public void GameOver(int winningteam)
    {
        whoWon = winningteam;
        

        foreach (GameObject player in ai_players)
        {
            player.GetComponentInChildren<AI_Weapon_Handler>().enabled = false;
            player.GetComponentInChildren<AI_Movement>().enabled = false;
            player.GetComponentInChildren<AI_Railgrinding>().enabled = false;
            player.GetComponentInChildren<AgentLinkMover>().enabled = false;
            player.GetComponentInChildren<NavMeshAgent>().enabled = false;
            player.GetComponentInParent<Respawn>().enabled = false;           

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
        }
        new_player.SetActive(false);
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == winningteam)
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
        
        end_cam.SetActive(true);
        gameover_screen.SetActive(true);
        gameover_text.text = " YOU WIN";
        win = true;
            if (check)
            {
                StartCoroutine(MoveForward());
                check = false;
                EndPlayers();
            }
        
        
    }
    bool check = true; //for some reason the coroutine keeps getting called twice
    void LoseScreen()
    {
        end_cam.SetActive(true);
        gameover_screen.SetActive(true);
        gameover_text.text = " YOU LOSE";
        
            if (check)
            {
                StartCoroutine(MoveForward());
                check = false;
                EndPlayers();
                
            }
        
        
    }
    float moveDistance = 5f;
    float moveDuration = 2f;
    private IEnumerator MoveForward()
    {
        // Calculate the start and forward positions
        Vector3 startPos = end_cam.transform.position;
        Vector3 forwardPos = startPos + end_cam.transform.forward * moveDistance;

        yield return StartCoroutine(MoveOverTime(startPos, forwardPos, moveDuration));
    }


    private IEnumerator MoveOverTime(Vector3 fromPos, Vector3 toPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = t * t * (3f - 2f * t);
            end_cam.transform.position = Vector3.Lerp(toPos, fromPos, smoothT);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            yield return null;
        }
        end_cam.transform.position = fromPos;
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FocusOnTarget());
    }
    Vector3 offset = new Vector3(0, 0, -5f);

    private IEnumerator FocusOnTarget()
    {
        Transform focusTarget = end_player.transform.GetChild(7);
        // Record where the camera starts
        Vector3 startPos = end_cam.transform.position;

        // Calculate the final position (offset from the target's position)
        Vector3 endPos = focusTarget.position + offset;

        // Calculate the final rotation by looking from the end position towards the target

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            Debug.Log("is this running");
            elapsed += Time.deltaTime;

            // Normalized time from 0 to 1
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // Optional: Use a simple ease-in/ease-out curve
            float smoothT = t * t * (3f - 2f * t);

            // Move smoothly
            end_cam.transform.position = Vector3.Lerp(startPos, endPos, smoothT);


            yield return null;
        }

        // Ensure final position/rotation is exact
        end_cam.transform.position = endPos;
    }



    void EndPlayers()
    {
        // Stop respawning, hide the main player
        Respawn.isOver = true;
        if (new_player != null)
            new_player.SetActive(false);

        // Use the end camera
        LookAtCamera.cam = end_cam.GetComponent<Camera>();
        AI_LookAt.cam = end_cam.GetComponent<Camera>();

        // Identify local player's position in the room list
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        int index = players.IndexOf(PhotonNetwork.LocalPlayer);

        // Grab spawn points for the end screen
        List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points;

        // --- Everyone spawns their own end‐game avatar ---
        Vector3 spawnPosition = points[0] + new Vector3(index * 5f, -2f, 3.8f);
        GameObject persistent_end_player = PhotonNetwork.Instantiate(
            player_prefab.name,
            spawnPosition,
            Quaternion.Euler(0, 180, 0)
        );

        // Determine local win/lose status and set animations
        int localTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        Animator localAnimator = persistent_end_player.GetComponentInChildren<Animator>();
        localAnimator.SetInteger("Win", (localTeam == whoWon) ? 1 : -1);

        // Disable normal gameplay scripts
        PlayerMovement pm = persistent_end_player.GetComponentInChildren<PlayerMovement>();
        Player_Health ph = persistent_end_player.GetComponentInChildren<Player_Health>();
        Weapon_Handler wh = persistent_end_player.GetComponentInChildren<Weapon_Handler>();
        Camera ownCam = persistent_end_player.GetComponentInChildren<Camera>();
        Canvas ownCanvas = persistent_end_player.GetComponentInChildren<Canvas>();
        Rigidbody rb = persistent_end_player.GetComponentInChildren<Rigidbody>();

        if (pm) pm.enabled = false;
        if (ph) ph.enabled = false;
        if (wh) wh.enabled = false;
        if (ownCam) ownCam.enabled = false;
        if (ownCanvas) ownCanvas.enabled = false;
        if (rb) rb.constraints = RigidbodyConstraints.FreezeAll;

        // --- CRUCIAL FIX: point 'end_player' to our newly spawned avatar ---
        end_player = persistent_end_player;

        // MasterClient also lines up and freezes the AI end screen:
        if (PhotonNetwork.IsMasterClient && ai_players != null)
        {
            int aiIndex = PhotonNetwork.PlayerList.Length;
            foreach (GameObject aiObj in ai_players)
            {
                aiObj.GetComponentInChildren<AI_Weapon_Handler>().enabled = false;
                aiObj.GetComponentInChildren<AI_Movement>().enabled = false;
                aiObj.GetComponentInChildren<AI_Railgrinding>().enabled = false;
                aiObj.GetComponentInChildren<AgentLinkMover>().enabled = false;
                aiObj.GetComponentInChildren<NavMeshAgent>().enabled = false;

                Rigidbody aiRb = aiObj.GetComponentInChildren<Rigidbody>();
                if (aiRb) aiRb.constraints = RigidbodyConstraints.FreezeAll;

                int aiTeam = aiObj.GetComponent<Team_Handler>().GetTeam();
                Animator aiAnim = aiObj.GetComponentInChildren<Animator>();
                aiAnim.SetInteger("Win", (aiTeam == whoWon) ? 1 : -1);

                Vector3 aiPosition = points[0] + new Vector3(aiIndex * 5f, -6f, 0f);
                aiObj.transform.position = aiPosition;
                aiObj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                aiIndex++;
            }
        }
    }




    public void BackToStart()
    {
        Respawn.isOver = false;
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        StartCoroutine(LeaveTeam());
        
        
    }
    public override void OnLeftRoom()
    {        
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
        AI_LookAt.cam = new_player.GetComponentInChildren<Camera>();
        AI_LookAt.pv = new_player.GetComponent<PhotonView>();




    }
   

    }



