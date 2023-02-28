using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Game_Manager : MonoBehaviourPunCallbacks
{
    public GameObject player_prefab;
    Vector3 position;
    [SerializeField] PhotonView pv;
    [SerializeField] int min_room_size;
    // Start is called before the first frame update
    void Awake() //make this it's own clickable function
    {
      
    }
    [PunRPC] public void SpawnPlayer()
    {
        Debug.Log("adding player");
        position = transform.position;
        PhotonNetwork.Instantiate(player_prefab.name, position, Quaternion.identity, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.PlayerList.Length >= min_room_size)
        {
            pv.RPC("SpawnPlayer", RpcTarget.All);
        }
    }
}
