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

    // Start is called before the first frame update
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        
        
        
        

    }
    private void Start()
    {
        Debug.Log(tm.GetTeamMembersCount(1));
        PhotonNetwork.LocalPlayer.JoinTeam(1);
        StartCoroutine(JoinTeam());
    }
    IEnumerator JoinTeam()
    {
        yield return new WaitUntil(() => tm.GetTeamMembersCount(1) > 0);
        Debug.Log(tm.GetTeamMembersCount(1));
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


