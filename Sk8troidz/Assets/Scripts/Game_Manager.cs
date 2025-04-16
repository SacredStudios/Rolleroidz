using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Game_Manager : MonoBehaviourPunCallbacks
{
    // --------------------------------------------------
    // Public / Serialized fields
    // --------------------------------------------------
    public GameObject player_prefab;
    public GameObject respawn_points;
    public GameObject lobby;
    public GameObject lobby_cam;
    public GameObject start_early;
    public GameObject gameover_screen;
    public GameObject end_screen;
    public GameObject end_cam;
    public GameObject AIPlayer;
    public GameObject AIPlayer2;
    public Chat_Manager chatManager;
    public Text list;
    public Text gameover_text;
    public Text countdown;

    [SerializeField] int min_room_size = 2;
    [SerializeField] int win_score = 15;

    // --------------------------------------------------
    // Private fields
    // --------------------------------------------------
    private bool game_ongoing = false;
    private bool check = true; // used to avoid double-calls in Win/Lose screen
    private bool win = false;
    private int team1count = 0;
    private int team2count = 0;
    private int whoWon = 1;
    private int count = 0; // used for AI count
    private PhotonView pv;
    private Weapon my_weapon;

    // References
    private GameObject new_player;         // The regular gameplay player
    private GameObject weapon_list;        // The stored weapon list
    private List<GameObject> ai_players;   // For AI

    // We’ll store the local "end-game" player here for the focus script
    private GameObject localEndPlayer;

    // For the camera move
    private float moveDistance = 5f;
    private float moveDuration = 2f;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        pv = GetComponent<PhotonView>();
        weapon_list = GameObject.Find("WeaponList");
        countdown.gameObject.SetActive(false);

        // Only the MasterClient sees the "start early" button
        if (!PhotonNetwork.IsMasterClient)
        {
            start_early.SetActive(false);
        }
    }

    void Start()
    {
        Respawn.isOver = false;
        pv.Owner.SetScore(0);
        // Cache your chosen weapon from the weapon list
        my_weapon = weapon_list.GetComponent<Weapon_List>().curr_weapon;

        // In 30 seconds, check if enough players joined
        Invoke("CheckForPlayersOnTime", 30f);
    }

    // --------------------------------------------------
    // Player Connect / Disconnect
    // --------------------------------------------------
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            chatManager.SendStatMessage(newPlayer.NickName + " entered the room.");
        }

        // Reset everyone's score
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            p.SetScore(0);
        }

        base.OnPlayerEnteredRoom(newPlayer);
        PropChange(); // refresh the player list

        // If we have enough people, start the game
        if (PhotonNetwork.CurrentRoom.PlayerCount >= min_room_size && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckForPlayers());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        PropChange(); // refresh the player list
    }

    // --------------------------------------------------
    // Start / Check For Players
    // --------------------------------------------------
    IEnumerator CheckForPlayers()
    {
        // Make sure there's at least min_room_size or skip for debugging
        yield return new WaitUntil(() =>
            PhotonNetwork.CurrentRoom.PlayerCount >= min_room_size
        /* or debugging: || 1 == 1 */
        );

        game_ongoing = true;

        // Show countdown on every client
        pv.RPC("Start_Countdown", RpcTarget.All);

        // Master spawns enough AI to fill up
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnAIPlayers();
        }

        // Wait 5 seconds for countdown to finish
        yield return new WaitForSeconds(5f);
        SpawnPlayers();
    }

    // Called after 30 second timer (if master hasn't pressed early start)
    public void CheckForPlayersOnTime()
    {
        if (start_early.activeSelf)
        {
            start_early.SetActive(false);
            StartCoroutine(CheckForPlayers());
        }
    }

    // Called if the Master presses the “start early” button
    public void CheckForPlayersDebug()
    {
        start_early.SetActive(false);
        StartCoroutine(CheckForPlayers());
    }

    [PunRPC]
    public void Start_Countdown()
    {
        countdown.gameObject.SetActive(true);
        StartCoroutine(CountdownCo(5));
    }

    IEnumerator CountdownCo(int n)
    {
        while (n > 0)
        {
            countdown.text = n.ToString();
            yield return new WaitForSeconds(1f);
            n--;
        }
        countdown.gameObject.SetActive(false);
    }

    // --------------------------------------------------
    // Spawning
    // --------------------------------------------------
    public void SpawnPlayers()
    {
        // We do a short “SwitchTeams” routine
        StartCoroutine(SwitchTeams());
    }

    IEnumerator SwitchTeams()
    {
        // Assign teams (1,2) in a simple alternating way
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        // If Master wants to do a forced shuffle, etc.
        int localCount = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            p.LeaveCurrentTeam();
            yield return new WaitUntil(() => p.GetPhotonTeam() == null);

            localCount++;
            byte assignedTeam = (localCount % 2 == 0) ? (byte)1 : (byte)2;
            p.JoinTeam(assignedTeam);
            yield return new WaitUntil(() => p.GetPhotonTeam() != null);
        }

        yield return new WaitForSeconds(1f);
        pv.RPC("SpawnPlayer", RpcTarget.All);
    }

    void SpawnAIPlayers()
    {
        ai_players = new List<GameObject>();

        // The total we want is min_room_size, so fill up with AI
        while (count < min_room_size - PhotonNetwork.PlayerList.Length)
        {
            count++;
            Vector3 position = Vector3.zero;

            // Alternate AI prefab just for variety
            GameObject ai = (count % 2 == 0)
                ? PhotonNetwork.Instantiate(AIPlayer2.name, position, Quaternion.identity, 0)
                : PhotonNetwork.Instantiate(AIPlayer.name, position, Quaternion.identity, 0);

            // Set references
            ai.GetComponentInChildren<AI_Weapon_Handler>().chat_manager = chatManager;
            ai.GetComponent<Respawn>().respawn_points
                = respawn_points.GetComponent<RespawnPoints>().respawn_points;

            // Place them at random spawn
            List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points;
            ai.transform.position = points[Random.Range(0, points.Count)];

            // The *actual* controlling “player” for AI is child(0)
            ai_players.Add(ai.transform.GetChild(0).gameObject);
        }
    }

    [PunRPC]
    public void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Once game is started, lock the room
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        // Hide UI 
        lobby_cam.SetActive(false);
        lobby.SetActive(false);

        // If local player's “new_player” isn’t already set
        if (new_player == null)
        {
            Vector3 position = transform.position;
            new_player = PhotonNetwork.Instantiate(
                player_prefab.name,
                position,
                Quaternion.identity,
                0
            );

            // Assign random spawn from the respawn list
            List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points;
            new_player.transform.position = points[Random.Range(0, points.Count)];

            // Attach references
            new_player.GetComponent<Respawn>().respawn_points
                = respawn_points.GetComponent<RespawnPoints>().respawn_points;

            new_player.GetComponentInChildren<Weapon_Handler>().weapon = my_weapon;
            new_player.GetComponentInChildren<Weapon_Handler>().weapon.chat_manager
                = chatManager;
        }

        // Let AI look at your local camera (so they face you)
        AI_LookAt.cam = new_player.GetComponentInChildren<Camera>();
        AI_LookAt.pv = new_player.GetComponent<PhotonView>();
    }

    // --------------------------------------------------
    // Scoring / Properties
    // --------------------------------------------------
    public override void OnPlayerPropertiesUpdate(
        Player targetPlayer,
        ExitGames.Client.Photon.Hashtable changedProps
    )
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Recalc scoreboard
            team1count = 0;
            team2count = 0;

            // Sum up player scores
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.GetPhotonTeam() != null)
                {
                    if (p.GetPhotonTeam().Code == 1) team1count += p.GetScore();
                    else team2count += p.GetScore();
                }
            }

            // Sum up AI players
            if (ai_players != null)
            {
                foreach (GameObject ai in ai_players)
                {
                    int aiTeam = ai.GetComponent<Team_Handler>().GetTeam();
                    int aiScore = ai.GetComponent<AI_Handler>().score;

                    if (aiTeam == 1) team1count += aiScore;
                    else team2count += aiScore;
                }
            }

            // Check for a winner
            if (team1count >= win_score || team2count >= win_score)
            {
                Invoke(nameof(DoubleCheck), 3f);
            }
        }

        PropChange();
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    void DoubleCheck()
    {
        // Double-check which team actually has the lead
        team1count = 0;
        team2count = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.GetPhotonTeam().Code == 1) team1count += p.GetScore();
            else team2count += p.GetScore();
        }

        if (ai_players != null)
        {
            foreach (GameObject ai in ai_players)
            {
                int aiTeam = ai.GetComponentInChildren<Team_Handler>().GetTeam();
                int aiScore = ai.GetComponentInChildren<AI_Handler>().score;

                if (aiTeam == 1) team1count += aiScore;
                else team2count += aiScore;
            }
        }

        if (team1count >= win_score) pv.RPC("GameOver", RpcTarget.All, 1);
        else if (team2count >= win_score) pv.RPC("GameOver", RpcTarget.All, 2);
    }

    [PunRPC]
    public void GameOver(int winningteam)
    {
        whoWon = winningteam; // store it

        // Hide normal movement
        if (new_player != null)
        {
            if (new_player.GetComponentInChildren<PlayerMovement>())
            {
                new_player.GetComponentInChildren<PlayerMovement>().enabled = false;
                new_player.GetComponentInChildren<Weapon_Handler>().enabled = false;
            }
            else
            {
                // Possibly your camera was detached?
                Camera c = new_player.GetComponentInChildren<Camera>();
                if (c) c.transform.parent = null;
            }
            new_player.SetActive(false);
        }

        // For AI, MasterClient disables them
        if (PhotonNetwork.IsMasterClient && ai_players != null)
        {
            foreach (GameObject ai in ai_players)
            {
                ai.GetComponentInChildren<AI_Weapon_Handler>().enabled = false;
                ai.GetComponentInChildren<AI_Movement>().enabled = false;
                ai.GetComponentInChildren<AI_Railgrinding>().enabled = false;
                ai.GetComponentInChildren<AgentLinkMover>().enabled = false;
                ai.GetComponentInChildren<NavMeshAgent>().enabled = false;
                ai.GetComponentInParent<Respawn>().enabled = false;
            }
        }

        // Show end‐game UI
        end_cam.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameover_screen.SetActive(true);

        // Determine if local player is on the winning team
        int localTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        if (localTeam == whoWon)
        {
            gameover_text.text = "YOU WIN";
            win = true;
        }
        else
        {
            gameover_text.text = "YOU LOSE";
            win = false;
        }

        // Actually spawn the local “end player” + freeze AI
        if (check)
        {
            check = false;
            EndPlayers();        // This spawns the local end‐game avatar
            StartCoroutine(MoveForward()); // This starts the camera motion
        }
    }

    // --------------------------------------------------
    // End Game Logic
    // --------------------------------------------------
    void EndPlayers()
    {
        Respawn.isOver = true;

        // Let the camera references be for end_cam
        LookAtCamera.cam = end_cam.GetComponent<Camera>();
        AI_LookAt.cam = end_cam.GetComponent<Camera>();

        // Find out local player's index so we can position them in a line
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        int index = players.IndexOf(PhotonNetwork.LocalPlayer);

        // Grab the spawn points for end
        List<Vector3> points = respawn_points.GetComponent<RespawnPoints>().respawn_points;
        if (points.Count == 0) points.Add(Vector3.zero);

        // Everyone spawns their own end‐avatar
        Vector3 spawnPos = points[0] + new Vector3(index * 5f, -2f, 3.8f);
        GameObject persistent_end_player = PhotonNetwork.Instantiate(
            player_prefab.name,
            spawnPos,
            Quaternion.Euler(0, 180, 0)
        );

        // Store reference so we can do FocusOnTarget
        localEndPlayer = persistent_end_player;

        // Win or lose animation
        int localTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        Animator localAnimator = persistent_end_player.GetComponentInChildren<Animator>();
        localAnimator.SetInteger("Win", (localTeam == whoWon) ? 1 : -1);

        // Turn off normal movement scripts
        var pm = persistent_end_player.GetComponentInChildren<PlayerMovement>();
        if (pm) pm.enabled = false;

        var ph = persistent_end_player.GetComponentInChildren<Player_Health>();
        if (ph) ph.enabled = false;

        var wh = persistent_end_player.GetComponentInChildren<Weapon_Handler>();
        if (wh) wh.enabled = false;

        // Turn off its camera and UI so it won't conflict with end_cam
        var ownCam = persistent_end_player.GetComponentInChildren<Camera>();
        if (ownCam) ownCam.enabled = false;

        var ownCanvas = persistent_end_player.GetComponentInChildren<Canvas>();
        if (ownCanvas) ownCanvas.enabled = false;

        // Freeze it in place
        Rigidbody rb = persistent_end_player.GetComponentInChildren<Rigidbody>();
        if (rb) rb.constraints = RigidbodyConstraints.FreezeAll;

        // Master moves the AI into position and locks them
        if (PhotonNetwork.IsMasterClient && ai_players != null)
        {
            int aiIndex = PhotonNetwork.PlayerList.Length;
            foreach (GameObject aiObj in ai_players)
            {
                // Freeze AI
                Rigidbody aiRb = aiObj.GetComponentInChildren<Rigidbody>();
                if (aiRb) aiRb.constraints = RigidbodyConstraints.FreezeAll;

                // Win or lose animation
                int aiTeam = aiObj.GetComponent<Team_Handler>().GetTeam();
                Animator aiAnim = aiObj.GetComponentInChildren<Animator>();
                aiAnim.SetInteger("Win", (aiTeam == whoWon) ? 1 : -1);

                // Position them in a row
                Vector3 aiPos = points[0] + new Vector3(aiIndex * 5f, -6f, 0f);
                aiObj.transform.position = aiPos;
                aiObj.transform.rotation = Quaternion.Euler(0, 180, 0);

                aiIndex++;
            }
        }
    }

    // --------------------------------------------------
    // Camera Move / Focus
    // --------------------------------------------------
    // Example coroutine to “pan forward” and then do final focus
    private IEnumerator MoveForward()
    {
        // Example forward pan
        Vector3 startPos = end_cam.transform.position;
        Vector3 forwardPos = startPos + end_cam.transform.forward * moveDistance;

        yield return StartCoroutine(MoveOverTime(startPos, forwardPos, moveDuration));

        // Wait a second or two, then focus on the end player
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FocusOnTarget());
    }

    private IEnumerator MoveOverTime(Vector3 fromPos, Vector3 toPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // a simple ease in/out
            float smoothT = t * t * (3f - 2f * t);

            end_cam.transform.position = Vector3.Lerp(fromPos, toPos, smoothT);
            yield return null;
        }
        // ensure final
        end_cam.transform.position = toPos;
    }

    private IEnumerator FocusOnTarget()
    {
        // if we don’t have a local end player, bail
        if (localEndPlayer == null)
            yield break;

        // Suppose child(7) is your “head” or “focus” transform
        Transform focusTarget = localEndPlayer.transform;
        // If you need a particular child, do:
        // Transform focusTarget = localEndPlayer.transform.GetChild(7);

        // We’ll do a nice little move from camera’s current pos
        // to behind the focusTarget + an offset
        Vector3 offset = new Vector3(0, 0, -5f);
        Vector3 startPos = end_cam.transform.position;
        Vector3 endPos = focusTarget.position + offset;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float smoothT = t * t * (3f - 2f * t);

            end_cam.transform.position = Vector3.Lerp(startPos, endPos, smoothT);

            // If you want the camera to lookAt the character,
            // do something like:
            end_cam.transform.LookAt(focusTarget);

            yield return null;
        }

        end_cam.transform.position = endPos;
        end_cam.transform.LookAt(focusTarget);
    }

    // --------------------------------------------------
    // Leaving the Room
    // --------------------------------------------------
    public void BackToStart()
    {
        Respawn.isOver = false;
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        StartCoroutine(LeaveTeam());
    }

    IEnumerator LeaveTeam()
    {
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPhotonTeam() == null);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("StartingScene");
    }

    // --------------------------------------------------
    // Helper
    // --------------------------------------------------
    void PropChange()
    {
        // update a simple Text listing players
        list.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            list.text += player.NickName + "\n";
        }
    }
}
