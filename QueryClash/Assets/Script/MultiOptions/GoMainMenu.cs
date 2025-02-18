using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMainMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuFNetwork");
    }
}