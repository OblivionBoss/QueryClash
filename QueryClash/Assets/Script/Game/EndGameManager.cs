using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Object;
using TMPro;

public class EndGameManager : NetworkBehaviour
{
    public Base leftBunker; // Assign the left bunker in the Inspector
    public Base rightBunker; // Assign the right bunker in the Inspector
    public Timer timer;
    public RDBManager rdbManager;

    private bool leftBunkerDestroyed = false;
    private bool rightBunkerDestroyed = false;
    private bool sentData = false; // To ensure the transition happens only once

    [SerializeField] private Canvas endGameMenu;
    [SerializeField] private TextMeshProUGUI winLoseDrawText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI querySuccessText;
    [SerializeField] private TextMeshProUGUI queryErrorText;

    [Server]
    public void Update()
    {
        if (!ClientManager.Connection.IsHost || !timer.isGameStart.Value || timer.isCountingDown.Value) return;
        // Check if the bunkers are destroyed
        if (leftBunker.CurrentHp.Value <= 0 && !leftBunkerDestroyed)
        {
            leftBunkerDestroyed = true;
            Debug.LogError("Left bunker destroyed");
        }

        if (rightBunker.CurrentHp.Value <= 0 && !rightBunkerDestroyed)
        {
            rightBunkerDestroyed = true;
            Debug.LogError("Right bunker destroyed");
        }

        // Check for the game-ending condition
        if (!sentData && (leftBunkerDestroyed || rightBunkerDestroyed))
        {
            sentData = true; // Prevent multiple triggers
            timer.isGameStart.Value = false;
            HandleEndGame();
        }
    }

    [Server]
    public void HandleEndGame()
    {
        if (!leftBunkerDestroyed && rightBunkerDestroyed) OpenEndMenu(true, false);         // left win (host)
        else if (leftBunkerDestroyed && !rightBunkerDestroyed) OpenEndMenu(false, false);   // right win (client)
        else if (leftBunkerDestroyed && rightBunkerDestroyed) OpenEndMenu(true, true);      // draw

        // Determine the order of destruction and pass the boolean to the next scene
        //bool rightFirst = rightBunkerDestroyed && !leftBunkerDestroyed;
        //PlayerPrefs.SetInt("RightDestroyedFirst", rightFirst ? 1 : 0);

        // Load the EndGame scene
        //SceneManager.LoadScene("EndGame");
    }

    [ObserversRpc]
    public void OpenEndMenu(bool hostWin, bool draw)
    {
        endGameMenu.gameObject.SetActive(true);

        bool isHost = ClientManager.Connection.IsHost;
        if (draw)
        {
            winLoseDrawText.text = "draw";
            winLoseDrawText.color = new Color(1f, 0.5f, 0f);
        }
        else if (hostWin && isHost || !hostWin && !isHost)
        {
            winLoseDrawText.text = "victory";
            winLoseDrawText.color = Color.green;
        }
        else if (!hostWin && isHost || hostWin && !isHost)
        {
            winLoseDrawText.text = "defeated";
            winLoseDrawText.color = Color.red;
        }

        // show stat from rdbmanager
        totalScoreText.text = rdbManager.queryStat.totalScore.ToString("#0");
        querySuccessText.text = rdbManager.queryStat.querySuccess.ToString();
        queryErrorText.text = rdbManager.queryStat.queryError.ToString();
    }

    public void GoToLobby()
    {
        if (ClientManager.Connection.IsHost)
        {
            ClientManager.StopConnection();
            ServerManager.StopConnection(true);
            ForceClientDisconnect();
        }
        else
        {
            ClientManager.StopConnection();
        }
        Destroy(GameObject.Find("NetworkManager"));
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameLobby");
    }

    [ObserversRpc]
    public void ForceClientDisconnect()
    {
        if (ClientManager.Connection.IsHost) return;

        ClientManager.StopConnection();
        Destroy(GameObject.Find("NetworkManager"));
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameLobby");
    }
}