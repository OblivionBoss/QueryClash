using UnityEngine;
using UnityEngine.SceneManagement;

public class BunkerManager : MonoBehaviour
{
    public GameObject leftBunker; // Assign the left bunker in the Inspector
    public GameObject rightBunker; // Assign the right bunker in the Inspector

    private bool leftBunkerDestroyed = false;
    private bool rightBunkerDestroyed = false;
    private bool sentData = false; // To ensure the transition happens only once

    void Update()
    {
        // Check if the bunkers are destroyed
        if (leftBunker == null && !leftBunkerDestroyed)
        {
            leftBunkerDestroyed = true;
            Debug.Log("Left bunker destroyed");
        }

        if (rightBunker == null && !rightBunkerDestroyed)
        {
            rightBunkerDestroyed = true;
            Debug.Log("Right bunker destroyed");
        }

        // Check for the game-ending condition
        if (!sentData && (leftBunkerDestroyed || rightBunkerDestroyed))
        {
            sentData = true; // Prevent multiple triggers
            HandleEndGame();
        }
    }

    void HandleEndGame()
    {
        // Determine the order of destruction and pass the boolean to the next scene
        bool rightFirst = rightBunkerDestroyed && !leftBunkerDestroyed;
        PlayerPrefs.SetInt("RightDestroyedFirst", rightFirst ? 1 : 0);

        // Load the EndGame scene
        SceneManager.LoadScene("EndGame");
    }
}

