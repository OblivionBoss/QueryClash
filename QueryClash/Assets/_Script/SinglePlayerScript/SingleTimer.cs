using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText; // Reference to the UI Text component
    public float elapsedTime = 0f; // Tracks the time after countdown ends
    public float countDown; // Duration of the countdown in seconds
    public bool isCountingDown = true; // Determines whether the countdown is active

    void Update()
    {
        if (isCountingDown)
        {
            // Decrease the countdown timer
            countDown -= Time.deltaTime;

            // Clamp to avoid negative values
            if (countDown <= 0)
            {
                countDown = 0;
                isCountingDown = false; // Switch to elapsed time mode
            }

            // Display the countdown timer
            int minutes = Mathf.FloorToInt(countDown / 60);
            int seconds = Mathf.FloorToInt(countDown % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // Start tracking and displaying elapsed time
            timerText.text = "Start";
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
