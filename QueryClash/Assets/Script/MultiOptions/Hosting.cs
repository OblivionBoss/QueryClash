using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hosting : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Awake()
    {
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
}