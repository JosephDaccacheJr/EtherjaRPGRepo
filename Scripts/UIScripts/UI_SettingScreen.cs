using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class UI_SettingScreen : MonoBehaviour
{
    public Button buttonLoad;
    private void OnEnable()
    {
        string savePath = Application.persistentDataPath + "/save.JSON";
        buttonLoad.interactable = File.Exists(savePath);

    }

    public void ClickedOnClose()
    {
        gameObject.SetActive(false);
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        SoundManager.PlaySound(SoundManager.instance.uiClose);
        UI_GameScreen.instance.ShowGameButtons();
    }

    public void ClickedOnSave()
    {
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        GameManager.instance.SaveGame();
    }

    public void ClickedOnLoad()
    {
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        GameManager.instance.LoadGame();
    }

    public void ClickOnQuit()
    {
        UI_GameScreen.instance.settingsScreen.SetActive(false);
        UI_GameScreen.instance.SetFadeScreen(true);
        Invoke("GoToMenu", 1f);
        MusicManager.instance.volumeExplore = 0f;
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
