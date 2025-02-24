using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBaseManager : MonoBehaviour
{
    //public SingleBase leftBase; // Assign the left bunker in the Inspector
    //public SingleBase rightBase; // Assign the right bunker in the Inspector
    //public SingleTimer timer;

    //private bool leftBunkerDestroyed = false;
    //private bool rightBunkerDestroyed = false;
    //private bool sentData = false; // To ensure the transition happens only once

    //void Update()
    //{
    //    if (!timer.isGameStart.Value || timer.isCountingDown.Value) return;
    //    // Check if the bunkers are destroyed
    //    if (leftBunker.CurrentHp.Value <= 0 && !leftBunkerDestroyed)
    //    {
    //        leftBunkerDestroyed = true;
    //        Debug.LogError("Left bunker destroyed");
    //    }

    //    if (rightBunker.CurrentHp.Value <= 0 && !rightBunkerDestroyed)
    //    {
    //        rightBunkerDestroyed = true;
    //        Debug.LogError("Right bunker destroyed");
    //    }

    //    // Check for the game-ending condition
    //    if (!sentData && (leftBunkerDestroyed || rightBunkerDestroyed))
    //    {
    //        sentData = true; // Prevent multiple triggers
    //        HandleEndGame();
    //    }
    //}

    //void HandleEndGame()
    //{
    //    // Determine the order of destruction and pass the boolean to the next scene
    //    bool rightFirst = rightBunkerDestroyed && !leftBunkerDestroyed;
    //    PlayerPrefs.SetInt("RightDestroyedFirst", rightFirst ? 1 : 0);

    //    // Load the EndGame scene
    //    SceneManager.LoadScene("EndGame");
    //}
}
