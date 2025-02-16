using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;

public class GetOppIP : MonoBehaviour
{
    public static GetOppIP getOppIP;
    public TMP_InputField inputField;
    public string Opp_IP;

    private NetworkManager _networkManager;

    private void Awake() {
        if (getOppIP == null) 
        { 
            getOppIP = this;
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionStateChange;
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

        //Opp_IP = "localhost";
        //InstanceFinder.NetworkManager.GetComponent<Tugboat>().SetClientAddress(Opp_IP);
        //InstanceFinder.NetworkManager.GetComponent<Tugboat>().SetClientAddress();
        //SceneManager.LoadSceneAsync("GameFNetwork"); //SceneManager.LoadSceneAsync("Game");
        //if (!OnStart_Client(Opp_IP))
        //{
        //    SceneManager.LoadSceneAsync("MainMenuFNetwork");
        //}
        //else SceneManager.LoadSceneAsync("GameFNetwork");
        OnStart_Client(Opp_IP);
        SceneManager.LoadScene("GameFNetwork");
    }

    public void OnStart_Client(string address)
    {
        if (_networkManager == null) return;
      
        StartCoroutine(ClientDelay());
        //_networkManager.ClientManager.StartConnection();
    }

    private IEnumerator ClientDelay()
    {
        yield return new WaitForSeconds(0.5f);

        _networkManager.ClientManager.StartConnection();
    }

    private void OnClientConnectionStateChange(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.LogError("Connection failed. Check server IP and try again.");
            SceneManager.LoadScene("MainMenuFNetwork");
        }
        else if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.LogError("Connected!");
        }
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