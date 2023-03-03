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
    
    [SerializeField] int min_room_size;
    [SerializeField] int team1size;
    [SerializeField] int team2size;
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject lobby_cam;
    [SerializeField] PhotonTeamsManager tm;
    [SerializeField] Text Team1List;
    [SerializeField] Text Team2List;
    [SerializeField] PhotonView pv;
    [SerializeField] int temp1 = 9999;
    [SerializeField] int temp2 = 9999;
    // Start is called before the first frame update
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        
        
        
        

    }
    private void Start()
    {
        AddToLobby();
        
    }
    void AddToLobby()
    {


        PhotonNetwork.LocalPlayer.JoinTeam((byte)Random.Range(1, 3));
        StartCoroutine(SwitchTeam(PhotonNetwork.LocalPlayer));
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
        }
        PropChange();
        
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
    void PropChange()
    {
        Debug.Log("propchange");
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

    
    [PunRPC] public void SpawnPlayerTeam1()
    {
        Debug.Log("adding player (1)");
        position = transform.position;
        GameObject new_player = PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
        new_player.GetComponent<Team>().team = true;
    }
    [PunRPC]
    public void SpawnPlayerTeam2(Player newPlayer)
    {
        Debug.Log("adding player (2)");
        position = transform.position;
        PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
        lobby.SetActive(false);
        lobby_cam.SetActive(false);
    }

    

    
        
        

    

    
    
    public void StartGame()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if (1==1)
            {
              //  pv.RPC("SpawnPlayerTeam1", RpcTarget.All);
                team1size++;
            }
            else
            {
              //  pv.RPC("SpawnPlayerTeam2", RpcTarget.All);
                team2size++;
            }
        }
    }
}


