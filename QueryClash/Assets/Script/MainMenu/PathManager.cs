using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play() { SceneManager.LoadSceneAsync("BossGameLobby"); } // Go to game lobby SceneManager.LoadSceneAsync("GameFNetwork");

    public void Home() { SceneManager.LoadSceneAsync("BossMainMenu"); } // Go to MainMenuSceneManager.LoadSceneAsync("MainMenuFNetwork");
    public void Tutorial() { SceneManager.LoadSceneAsync("BossGameLobby"); } // Go to Turorial
    public void Singleplayer() { SceneManager.LoadSceneAsync("BossGameLobby"); } // Go to Singleplayer

    public void Multiplayer() { SceneManager.LoadSceneAsync("BossMultiOptions"); } // Go to MultiOptions
    public void Quit() { Application.Quit(); } // Terminated application
}
