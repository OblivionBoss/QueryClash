using UnityEngine;
using TMPro;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;

//Made by Bobsi Unity - for youtube
public class ChatBroadcastGuide : MonoBehaviour
{
    public Transform chatHolder;
    public GameObject msgElement;
    public TMP_InputField playerUsername, playerMessage;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageRecieved); //OnMessageRecieved
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageRecieved); //OnClientMessageRecieved
    }
    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageRecieved);
        InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageRecieved);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    private void SendMessage()
    {
        Message msg = new Message()
        {
            username = playerUsername.text,
            message = playerMessage.text
        };

        playerMessage.text = "";

        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Broadcast(msg);
        else if (InstanceFinder.IsClientStarted)
            InstanceFinder.ClientManager.Broadcast(msg);
    }

    private void OnMessageRecieved(Message msg, Channel channel)
    {
        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        if(msg.message != "") finalMessage.GetComponent<TextMeshProUGUI>().text = msg.username + ": " + msg.message;
    }

    private void OnClientMessageRecieved(NetworkConnection networkConnection, Message msg, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast
    {
        public string username;
        public string message;
    }
}