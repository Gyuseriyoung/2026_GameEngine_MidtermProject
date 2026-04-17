using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelObject : MonoBehaviour
{
    public string nextlevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void MoveToNextLevel()
    {
        SceneManager.LoadScene(nextlevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
