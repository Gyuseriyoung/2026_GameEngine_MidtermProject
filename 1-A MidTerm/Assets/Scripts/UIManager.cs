using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    
    public void GameStartButtonAction()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void GameMainMenuButtonAction()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

}
