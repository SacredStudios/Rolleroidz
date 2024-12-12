using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

public class Chat_Manager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject chatIcon;
    string currChat;
    [SerializeField] InputField inputfield;
    [SerializeField] Text chatDisplay;


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
        Debug.Log("getting msgs");
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);
            chatDisplay.text += "\n "+ messages[i];
            Debug.Log(msgs);
        }
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
    public void SubmitChat()
    {
        //Add logic for private chat here
        if (currChat != "")
        {
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
            {
                if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 1)
                {
                    chatClient.PublishMessage("RegionChannel", "<color=red>" + PhotonNetwork.NickName + ": " + currChat + "</color>");
                }
                else
                {
                    chatClient.PublishMessage("RegionChannel", "<color=#BA13FF>" + PhotonNetwork.NickName + ": " + currChat + "</color>");
                }
            }
            else
            {
                chatClient.PublishMessage("RegionChannel", PhotonNetwork.NickName + ": " + currChat);
            }
            inputfield.text = "";
            currChat = "";
        }
        
    }
    public void TypeChatOnValueChange(string value)
    {
        currChat = value;
    }

    public void OpenChat()
    {
        chatPanel.SetActive(true);
        chatIcon.SetActive(false);
    }
    public void CloseChat()
    {
        chatPanel.SetActive(false);
        chatIcon.SetActive(true);
    }
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitChat();
            }
         //   Debug.Log("ChatClient State: " + chatClient.State);
        }
    }

}
