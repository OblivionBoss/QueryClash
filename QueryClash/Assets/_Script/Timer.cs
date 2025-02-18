using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Timer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText; // Reference to the UI Text component
    public readonly SyncVar<float> elapsedTime = new SyncVar<float>(0f); // Tracks the time after countdown ends
    public readonly SyncVar<float> countDown = new SyncVar<float>(15f); // Duration of the countdown in seconds
    public readonly SyncVar<bool> isCountingDown = new SyncVar<bool>(true); // Determines whether the countdown is active
    public readonly SyncVar<bool> isGameStart = new SyncVar<bool>(true); // Determines whether the game is start

    [Server]
    void Update()
    {
        if (!isGameStart.Value) return;

        if (ClientManager.Connection.IsHost) UpdateTime();

        if (isCountingDown.Value)
        {
            // Display the countdown timer
            DisplayTime(countDown.Value);
        }
        else
        {
            // Display the elapsed time
            DisplayTime(elapsedTime.Value);
        }
    }

    [Server]
    public void UpdateTime()
    {
        if (isCountingDown.Value)
        {
            // Decrease the countdown timer
            countDown.Value -= Time.deltaTime;

            // Clamp to avoid negative values
            if (countDown.Value <= 0)
            {
                countDown.Value = 0;
                isCountingDown.Value = false; // Switch to elapsed time mode
            }
        }
        else
        {
            // Start tracking elapsed time
            elapsedTime.Value += Time.deltaTime;
        }
    }

    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
