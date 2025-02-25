using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play()  // Go to game lobby SceneManager.LoadSceneAsync("GameFNetwork");
    {
        GameObject nm = GameObject.Find("NetworkManager");
        if (nm != null) Destroy(nm);
        SceneManager.LoadSceneAsync("BossGameLobby");
    }

    public void Home() { SceneManager.LoadSceneAsync("BossMainMenu"); } // Go to MainMenu SceneManager.LoadSceneAsync("MainMenuFNetwork");

    public void Tutorial() { SceneManager.LoadSceneAsync("BossGameLobby"); } // Go to Turorial

    public void Singleplayer() { SceneManager.LoadSceneAsync("SingleLobby"); } // Go to Singleplayer

    public void Multiplayer() { SceneManager.LoadSceneAsync("BossMultiOptions"); } // Go to MultiOptions

    public void Quit() { Application.Quit(); } // Terminated application
}
