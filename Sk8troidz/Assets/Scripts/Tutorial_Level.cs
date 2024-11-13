using Photon.Pun;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject player_assets;
    GameObject WeaponList;

    void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {

        WeaponList = GameObject.Find("WeaponList");
        GameObject player = PhotonNetwork.Instantiate(player_assets.name, this.transform.position, this.transform.rotation);
        player.GetComponentInChildren<Weapon_Handler>().weapon = WeaponList.GetComponent<Weapon_List>().curr_weapon;
        player.GetComponent<PhotonView>().Owner.JoinTeam(1);
        //player.transform.GetChild(3).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room, creating a new room.");
        PhotonNetwork.CreateRoom(null);
    }
}
