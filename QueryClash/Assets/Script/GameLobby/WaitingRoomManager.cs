using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using System.Net;
using System.Linq;
using TMPro;

public class WaitingRoomManager : NetworkBehaviour
{
    [SerializeField] private Canvas waitingRoom;
    [SerializeField] private Button startGameHost;
    [SerializeField] private Button readyClient;
    [SerializeField] private TextMeshProUGUI hostIPText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!ClientManager.Connection.IsHost)
        {
            startGameHost.gameObject.SetActive(false);
            readyClient.gameObject.SetActive(true);
            hostIPText.gameObject.SetActive(false);
        }
        else
        {
            hostIPText.text = "Host IP : " + GetLocalIPv4();
        }
    }

    public void StartGame()
    {
        waitingRoom.gameObject.SetActive(false);
    }

    private string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }
}