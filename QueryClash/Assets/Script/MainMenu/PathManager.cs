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

        GameObject ssm = GameObject.Find("SingleSceneManager");
        if (ssm != null) Destroy(ssm);

        SceneManager.LoadSceneAsync("GameLobby");
    }

    public void Home() { SceneManager.LoadSceneAsync("MainMenu"); } // Go to MainMenu SceneManager.LoadSceneAsync("MainMenuFNetwork");

    public void Tutorial() { SceneManager.LoadSceneAsync("GameLobby"); } // Go to Turorial

    public void Singleplayer() { SceneManager.LoadSceneAsync("SingleLobby"); } // Go to Singleplayer

    public void Multiplayer() { SceneManager.LoadSceneAsync("MultiOption"); } // Go to MultiOptions

    public void Quit() { Application.Quit(); } // Terminated application
}
