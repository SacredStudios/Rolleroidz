using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject player_assets;

    void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Attempting to join a random room...");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room, now instantiating player.");
        PhotonNetwork.Instantiate(player_assets.name, this.transform.position, this.transform.rotation);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room, creating a new room.");
        PhotonNetwork.CreateRoom(null);
    }
}
