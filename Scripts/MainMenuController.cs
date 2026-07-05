using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Image fade;
    bool fadeOut;
    public GameObject buttonNew, buttonContinue, buttonQuit;
    public AudioSource menuMusic;
    public AudioSource sfxClick;

    private void Awake()
    {
        fade.color = new Color(0f, 0f, 0f, 1f);
    }

    void Start()
    {
        
    }

    void Update()
    {
        float fadeTarget = fadeOut ? 1f : 0f;
        fade.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(fade.color.a, fadeTarget, Time.deltaTime * 1f));
        menuMusic.volume = 0.5f - (fade.color.a / 2f);
    }

    public void ClickContinue()
    {
        sfxClick.Play();
        DisableButtons();
        fadeOut = true;

        Invoke("LoadContinue", 1f);

    }

    public void ClickNew()
    {
        sfxClick.Play();
        DisableButtons();
        fadeOut = true;

        Invoke("NewGame", 1f);
    }

    public void ClickQuit()
    {
        sfxClick.Play();
        DisableButtons();
        fadeOut = true;

        Invoke("QuitGame", 1f);
    }

    void DisableButtons()
    {
        buttonNew.SetActive(false);
        buttonContinue.SetActive(false);
        buttonQuit.SetActive(false);
    }

    void LoadContinue()
    {
        SceneController.instance.ContinueGame();
    }

    void NewGame()
    {
        SceneController.instance.StartNewGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
