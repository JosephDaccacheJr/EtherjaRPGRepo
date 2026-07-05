using UnityEngine;
using UnityEngine.UI;
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
}
