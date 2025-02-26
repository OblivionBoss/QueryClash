using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseManager : MonoBehaviour
{
    public SingleBase leftBunker; // Assign the left bunker in the Inspector
    public SingleBase rightBunker; // Assign the right bunker in the Inspector
    public SingleTimer timer;
    public RDBManager rdbManager;

    private bool leftBunkerDestroyed = false;
    private bool rightBunkerDestroyed = false;
    private bool sentData = false; // To ensure the transition happens only once

    [SerializeField] private Canvas endGameMenu;
    [SerializeField] private TextMeshProUGUI winLoseDrawText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI querySuccessText;
    [SerializeField] private TextMeshProUGUI queryErrorText;

    void Update()
    {
        if (timer.isCountingDown) return;
        // Check if the bunkers are destroyed
        if (leftBunker.CurrentHp <= 0 && !leftBunkerDestroyed)
        {
            leftBunkerDestroyed = true;
        }

        if (rightBunker.CurrentHp <= 0 && !rightBunkerDestroyed)
        {
            rightBunkerDestroyed = true;
        }

        // Check for the game-ending condition
        if (!sentData && (leftBunkerDestroyed || rightBunkerDestroyed))
        {
            sentData = true; // Prevent multiple triggers
            //timer.isGameStart = false;
            HandleEndGame();
        }
    }

    public void HandleEndGame()
    {
        endGameMenu.gameObject.SetActive(true);
        if (!leftBunkerDestroyed && rightBunkerDestroyed)   // left win (player)
        {
            winLoseDrawText.text = "victory";
            winLoseDrawText.color = Color.green;
        }
        else if (leftBunkerDestroyed && !rightBunkerDestroyed) // right win (bot)
        {
            winLoseDrawText.text = "defeated";
            winLoseDrawText.color = Color.red;
        }
        else if (leftBunkerDestroyed && rightBunkerDestroyed) // draw
        {
            winLoseDrawText.text = "draw";
            winLoseDrawText.color = new Color(1f, 0.5f, 0f);
        }

        // show stat from rdbmanager
        totalScoreText.text = rdbManager.queryStat.totalScore.ToString("#0");
        querySuccessText.text = rdbManager.queryStat.querySuccess.ToString();
        queryErrorText.text = rdbManager.queryStat.queryError.ToString();
    }

    public void GoToLobby()
    {
        Destroy(GameObject.Find("SingleSceneManager"));
        SceneManager.LoadSceneAsync("GameLobby");
    }
}