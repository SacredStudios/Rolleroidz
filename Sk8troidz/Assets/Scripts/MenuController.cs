using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MenuController : MonoBehaviourPunCallbacks
{
    public GameObject player_prefab;
    Vector3 position;
    [SerializeField] GameObject transition;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 target_pos;
    [SerializeField] Vector3 start_pos;

    [SerializeField] GameObject StartBtn;
    [SerializeField] GameObject Menu_Skatroid;
    [SerializeField] GameObject StartMenu;
    [SerializeField] int max_players;


    public void ShowMainMenu()
    {
        StartBtn.GetComponent<Button>().enabled = false;
       // StartCoroutine(ConnectToServer());
        StartCoroutine(Transition_Down());   
        Invoke("HideStartMenu", 1f);

    }
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = "NewPlayer";
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        start_pos = transition.transform.position;
        //DontDestroyOnLoad(this.gameObject);

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }
    void HideStartMenu()
    {
        StartMenu.SetActive(false);
        Menu_Skatroid.SetActive(true);
    }

    IEnumerator Transition_Down()
    {
        
        while (transition.transform.position.y > target_pos.y)
        {
            transition.transform.position -= velocity;

            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        transition.transform.position = new Vector3(transition.transform.position.x, target_pos.y, transition.transform.position.z);
        StartCoroutine(Transition_Up());

    }
    IEnumerator Transition_Up()
    {
        
        while (transition.transform.position.y < start_pos.y)
        {
            transition.transform.position += velocity;
            Debug.Log("goin up");
            yield return new WaitForSeconds(0.01f);
        }
        transition.transform.position = start_pos;

    }
    public void ChangeUsername(string s)
    {
        
        int white_spaces = 0;// = s.Length(char.IsWhiteSpace)
        for(int i = 0; i < s.Length; i++)
        {

            if (char.IsWhiteSpace(s[i]))
                white_spaces++;
        }
        if (s.Length-white_spaces >= 1)
        {
            PhotonNetwork.NickName = s;
        }
        
    }
    public void AddRandomGame()
    {
   
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        //PhotonNetwork.JoinRandomOrCreateRoom(null, roomOptions.MaxPlayers, MatchmakingMode.FillRoom, TypedLobby.Default);
        StartCoroutine(LeaveTeam());
        
    }
    IEnumerator LeaveTeam()
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPhotonTeam() == null);
       
        PhotonNetwork.JoinRandomRoom();
        

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)max_players;
        roomOptions.EmptyRoomTtl = 0;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("DebugRoom");

    }
    
}