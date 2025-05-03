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
    [SerializeField] Animation transition_start;
    [SerializeField] Animation transition_end;
    [SerializeField] Text ConnectedText;
    [SerializeField] InputField input_field;

    [SerializeField] GameObject play_online_btn;
    [SerializeField] GameObject tutorial_btn;


    public void ShowMainMenu()
    {      
        StartBtn.GetComponent<Button>().enabled = false;
       // StartCoroutine(ConnectToServer());
        Transition();   
        Invoke("HideStartMenu", 1f);

    }
    private void Awake()
    {
        play_online_btn.GetComponent<Button>().interactable = false;
        tutorial_btn.GetComponent<Button>().interactable = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = "NewPlayer";
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        start_pos = transition.transform.position;
        if (PlayerPrefs.HasKey("nickname"))
        {
            input_field.text = PlayerPrefs.GetString("nickname");
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Default controls WASD
        string LeftKeyPref = "LeftKey";
        string RightKeyPref = "RightKey";
        string UpKeyPref = "UpKey";
        string DownKeyPref = "DownKey";
        string ShootKeyPref = "ShootKey";
        string JumpKeyPref = "JumpKey";
        string TrickKeyPref = "TrickKey";


        if (!PlayerPrefs.HasKey(LeftKeyPref)) PlayerPrefs.SetInt(LeftKeyPref, (int)KeyCode.A);
        if (!PlayerPrefs.HasKey(RightKeyPref)) PlayerPrefs.SetInt(RightKeyPref, (int)KeyCode.D);
        if (!PlayerPrefs.HasKey(UpKeyPref)) PlayerPrefs.SetInt(UpKeyPref, (int)KeyCode.W);
        if (!PlayerPrefs.HasKey(DownKeyPref)) PlayerPrefs.SetInt(DownKeyPref, (int)KeyCode.S);
        if (!PlayerPrefs.HasKey(JumpKeyPref)) PlayerPrefs.SetInt(JumpKeyPref, (int)KeyCode.Space);
        if (!PlayerPrefs.HasKey(ShootKeyPref)) PlayerPrefs.SetInt(ShootKeyPref, (int)KeyCode.E);
        if (!PlayerPrefs.HasKey(TrickKeyPref)) PlayerPrefs.SetInt(TrickKeyPref, (int)KeyCode.T);
        PlayerPrefs.Save();
    }
   
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        ConnectedText.text = "Connected to Server!";
        ConnectedText.color = new Color(0.45f,1f,0.45f);
        play_online_btn.GetComponent<Button>().interactable = true;
        tutorial_btn.GetComponent<Button>().interactable = true;
    }
    void HideStartMenu()
    {
        StartMenu.SetActive(false);
        Menu_Skatroid.SetActive(true);
        
    }

    public void Transition()
    {
        if (transition != null)
        {
            transition.SetActive(true);
            transition.GetComponent<Animator>().SetBool("BothAnim", true);
        }

    }
    public void SetInactive()
    {
        transition.SetActive(false);
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
            PlayerPrefs.SetString("nickname", s);

        }
        
    }
    public void AddRandomGame()
    {

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        StartCoroutine(LeaveTeam());
        
        
    }
    IEnumerator LeaveTeam()
    {
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPhotonTeam() == null);
        }
       
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
        transition.SetActive(true);
        PhotonNetwork.LocalPlayer.JoinTeam((byte)Random.Range(1, 3));
        transition.GetComponent<Animator>().SetBool("BothAnim", false);
        Invoke("LoadLevel", 1f);        
    }
    public void LoadLevel()
    {
        PhotonNetwork.LoadLevel("DebugRoom");
    }
    public void LoadTutorial()
    {
        Invoke("OpenTutorial", 0.5f);
    }
    public void OpenTutorial()
    {
        PhotonNetwork.LoadLevel("TutorialRoom");
    }

}