using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    public void PlayGame()
    {
        // loads the next scene in the queue (ie, the game)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("User has quit the game!");
        Application.Quit();
    }
}
