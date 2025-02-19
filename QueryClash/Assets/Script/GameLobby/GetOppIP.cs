using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Managing.Client;

public class GetOppIP : MonoBehaviour
{
    public static GetOppIP getOppIP;
    public TMP_InputField inputField;
    public string Opp_IP;

    private NetworkManager _networkManager;
    private ClientManager _clientManager;
    public Tugboat TB;

    private void Awake() {
        if (getOppIP == null) 
        { 
            getOppIP = this;
            _clientManager = InstanceFinder.ClientManager;
            _clientManager.OnClientConnectionState += OnClientConnectionStateChange;
            _clientManager.OnClientTimeOut += OnClientTimeOut;
            DontDestroyOnLoad(gameObject); 
        }
        else if (getOppIP != this)
        {
            Destroy(getOppIP.gameObject);
            getOppIP = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        if (_networkManager == null) _networkManager = InstanceFinder.NetworkManager;
    }

    public void StartHosting()
    {
        OnClick_Host();
        SceneManager.LoadSceneAsync("GameFNetwork");
    }
    public void OnClick_Host()
    {
        if (_networkManager == null) return;
        StartCoroutine(HostingDelay());
    }

    private IEnumerator HostingDelay()
    {
        yield return new WaitForSeconds(0.5f);

        _networkManager.ServerManager.StartConnection();

        _networkManager.ClientManager.StartConnection("localhost");
    }

    public void GetIPv4() { 
        Opp_IP = inputField.text;
        if (Opp_IP.Equals(string.Empty))
        {
            Debug.LogError("ip must not empty");
            return;
        }
        
        OnStart_Client();
        SceneManager.LoadSceneAsync("GameFNetwork");
    }

    public void OnStart_Client()
    {
        if (_networkManager == null) return;

        if (TB == null) TB = _networkManager.GetComponent<Tugboat>();
        TB.SetClientAddress(Opp_IP);
        if (TB != null) Debug.LogError("TB != null");
        Debug.LogError("ip = " + Opp_IP);
        StartCoroutine(ClientDelay());
    }

    private IEnumerator ClientDelay()
    {
        yield return new WaitForSeconds(0.5f);

        _networkManager.ClientManager.StartConnection(Opp_IP);
        Debug.LogError("ip cdelay = " + Opp_IP);
        Debug.LogError("After startconn" + TB.GetClientAddress());
    }

    private void OnClientConnectionStateChange(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.LogError("Connection Stop.");
            SceneManager.LoadSceneAsync("MainMenuFNetwork");
            //SceneManager.LoadScene("BossGameLobby");
        }
        else if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.LogError("Connected!");
        }
    }

    private void OnClientTimeOut()
    {
        Debug.LogError("Connection Timeout. Check server IP and try again.");
        //SceneManager.LoadScene("MainMenuFNetwork");
        SceneManager.LoadSceneAsync("BossGameLobby");
    }
}

//public void OnClick_Host()
//{
//    if (_networkManager == null) return;
//    StartCoroutine(HostingDelay());
//}

//private IEnumerator HostingDelay()
//{
//    yield return new WaitForSeconds(0.5f);

//    if (_serverState != LocalConnectionState.Stopped)
//        _networkManager.ServerManager.StopConnection(true);
//    else
//        _networkManager.ServerManager.StartConnection();

//    if (_clientState != LocalConnectionState.Stopped)
//        _networkManager.ClientManager.StopConnection();
//    else
//        _networkManager.ClientManager.StartConnection("localhost");
//}

//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement;
//using FishNet.Transporting.Tugboat;
//using FishNet;
//using System.Net;
//using System.Net.Sockets;
//using System.Net.NetworkInformation;

//public class GetOppIP : MonoBehaviour
//{
//    public static GetOppIP getOppIP;
//    public TMP_InputField inputField;
//    public string Opp_IP;

//    private void Awake()
//    {
//        if (getOppIP == null) { getOppIP = this; DontDestroyOnLoad(gameObject); }
//        else { Destroy(gameObject); }
//    }

//    public void GetIPv4()
//    {
//        Opp_IP = inputField.text;
//        SceneManager.LoadSceneAsync("BossWaitingRoom");//SceneManager.LoadSceneAsync("GameFNetwork");
//    }

//    public void HostGame()
//    {
//        Opp_IP = GetLocalIPv4(); // Get the local IPv4 address
//        inputField.text = Opp_IP; // Update input field (optional)
//        Debug.Log("Hosting Game with IP: " + Opp_IP);

//        // Load the game scene as a host
//        SceneManager.LoadSceneAsync("BossWaitingRoom");//SceneManager.LoadSceneAsync("GameFNetwork");
//    }

//    private string GetLocalIPv4()
//    {
//        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
//        {
//            // Ignore virtual adapters (like ZeroTier)
//            if (ni.Description.ToLower().Contains("zerotier") || ni.Name.ToLower().Contains("zerotier"))
//                continue;

//            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
//                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
//            {
//                if (ni.OperationalStatus == OperationalStatus.Up)
//                {
//                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
//                    {
//                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
//                        {
//                            return ip.Address.ToString();
//                        }
//                    }
//                }
//            }
//        }
//        return "127.0.0.1"; // Default to localhost if no valid IP found
//    }
//}