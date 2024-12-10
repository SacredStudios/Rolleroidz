using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Chat_Manager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    [SerializeField] GameObject chatPanel;

    public void DebugReturn(DebugLevel level, string message)
    {
   
    }

    public void OnChatStateChange(ChatState state)
    {
     //   throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        Debug.Log("connected to chat!");
        chatClient.Subscribe(new string[] { "RegionChannel" });
    }

    public void OnDisconnected()
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("is this running");
        chatPanel.SetActive(true);
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    void Start()
    {
        Debug.Log("AppIdChat: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat);
        chatClient = new ChatClient(this);
        bool connectionResult = chatClient.Connect(
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion,
            new AuthenticationValues(PhotonNetwork.NickName)
        );

        if (connectionResult)
        {
            Debug.Log("ChatClient is attempting to connect...");
        }
        else
        {
            Debug.LogError("ChatClient failed to initiate connection. Check AppIdChat or network settings.");
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
         //   Debug.Log("ChatClient State: " + chatClient.State);
        }
    }

}
