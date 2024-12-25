using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play() { SceneManager.LoadSceneAsync("GameLobby"); } // Go to game lobby

    public void Quit() { Application.Quit(); } // Terminated application
}
