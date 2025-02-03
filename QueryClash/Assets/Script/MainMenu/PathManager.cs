using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play() { SceneManager.LoadSceneAsync("GameFNetwork"); } // Go to game lobby SceneManager.LoadSceneAsync("GameLobby");

    public void Home() { SceneManager.LoadSceneAsync("MainMenuFNetwork"); } // Go to MainMenu
    public void Quit() { Application.Quit(); } // Terminated application
}
