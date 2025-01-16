using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play() { SceneManager.LoadSceneAsync("Game"); } // Go to game lobby SceneManager.LoadSceneAsync("GameLobby");

    public void Home() { SceneManager.LoadSceneAsync("MainMenu"); } // Go to MainMenu
    public void Quit() { Application.Quit(); } // Terminated application
}
