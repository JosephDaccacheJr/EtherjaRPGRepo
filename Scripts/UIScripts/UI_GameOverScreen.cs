using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOverScreen : MonoBehaviour
{
    public void ClickLoad()
    {
        GameManager.instance.LoadGame();
        UI_GameScreen.instance.ShowGameButtons();
        gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
