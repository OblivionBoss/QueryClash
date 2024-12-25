using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play() {  } // Go to game lobby SceneManager.LoadSceneAsync("GameLobby");

    public void Quit() { Application.Quit(); } // Terminated application
}
