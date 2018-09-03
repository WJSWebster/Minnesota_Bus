using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public GameController gameController;

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

    public void ReturnToMenu()
    {
        // loads the previous scene in the queue (ie, the main menu)
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        //gameController = gameController.GetComponent<GameController>();
        gameController.ResetGame();
    }
}
