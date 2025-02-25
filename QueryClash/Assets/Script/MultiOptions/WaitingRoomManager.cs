using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using System.Net;
using System.Linq;
using TMPro;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Diagnostics;

public class WaitingRoomManager : NetworkBehaviour
{
    [SerializeField] private Canvas waitingRoom;
    [SerializeField] private Canvas gameUI;
    [SerializeField] private Button startGameHost;
    [SerializeField] private Button readyClient;
    [SerializeField] private Image loadingScene;
    [SerializeField] private TextMeshProUGUI hostIPText;
    [SerializeField] private TextMeshProUGUI numberOfPlayerText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private TMP_Dropdown rdbDropdown;
    public readonly SyncVar<bool> isReady = new SyncVar<bool>(false);
    public readonly SyncVar<int> connNumber = new SyncVar<int>(0);
    public readonly SyncVar<int> seed = new SyncVar<int>(42);
    public readonly SyncVar<int> rdb = new SyncVar<int>(0);

    [SerializeField] private QueryMaterialManager queryMaterialManager;
    [SerializeField] private RDBManager rdbManager;
    [SerializeField] private Timer timer;
    [SerializeField] private string[] rdbList;

    public override void OnStartClient()
    {
        base.OnStartClient();
        gameUI.enabled = false;
        if (!ClientManager.Connection.IsHost)
        {
            startGameHost.gameObject.SetActive(false);
            readyClient.gameObject.SetActive(true);
            hostIPText.gameObject.SetActive(false);
            rdbDropdown.gameObject.SetActive(false);
        }
        else
        {
            startGameHost.gameObject.SetActive(true);
            readyClient.gameObject.SetActive(false);
            hostIPText.gameObject.SetActive(true);
            rdbDropdown.gameObject.SetActive(true);

            hostIPText.text = "Host IP : " + GetLocalIPv4();
            startGameHost.interactable = false;
            connNumber.OnChange += ConnNumber_OnChange;

            seed.Value = (int)(Stopwatch.GetTimestamp() % int.MaxValue);
        }

        isReady.OnChange += OnReady;
        StartCoroutine(DelayLoading());
    }

    private IEnumerator DelayLoading()
    {
        yield return new WaitForSeconds(0.5f);

        loadingScene.gameObject.SetActive(false);
    }

    private void ConnNumber_OnChange(int prev, int next, bool asServer)
    {
        if (prev == 2 && next != 2) isReady.Value = false;
    }

    private void OnReady(bool prev, bool next, bool asServer)
    {
        if (isReady.Value)
        {
            readyText.text = "Ready";
            readyText.color = new Color(0.329571f, 1f, 0f);
            startGameHost.interactable = true;
        }
        else
        {
            readyText.text = "Not Ready";
            readyText.color = new Color(1f, 0f, 0f);
            startGameHost.interactable = false;
        }
    }

    private void Update()
    {
        if (ClientManager.Connection.IsHost) setConnNum();
        numberOfPlayerText.text = $"Player Connection : {connNumber.Value} / 2";
    }

    [Server]
    private void setConnNum()
    {
        connNumber.Value = ServerManager.Clients.Count;
    }

    [Server]
    public void StartGame()
    {
        if (!ClientManager.Connection.IsHost) return;
        if (!isReady.Value) return;

        timer.isGameStart.Value = true;
        StartGameClient();
    }

    [ObserversRpc]
    public void StartGameClient()
    {
        waitingRoom.gameObject.SetActive(false);
        gameUI.enabled = true;
        queryMaterialManager.SetRandomInitStateNetwork(seed.Value);

        if (rdb.Value < rdbList.Length)
            rdbManager.StartRDB(rdbList[rdb.Value]);
        else
            rdbManager.StartRDB(rdbList[0]);
    }

    public void OnClickReady()
    {
        if (ClientManager.Connection.IsHost) return;

        OnReadyServer();
        readyClient.interactable = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnReadyServer()
    {
        if (!isReady.Value) isReady.Value = true;
    }

    [Server]
    public void OnChooseRDB()
    {
        if (!ClientManager.Connection.IsHost) return;

        rdb.Value = rdbDropdown.value;
    }

    public void GoBack()
    {
        if (ClientManager.Connection.IsHost)
        {
            ClientManager.StopConnection();
            ServerManager.StopConnection(true);
        }
        else
        {   
            ClientManager.StopConnection();
        }
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BossMultiOptions");
    }

    //private IEnumerator StopConnClientDelay()
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    ClientManager.StopConnection();
    //    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuFNetwork");
    //}

    private string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }
}