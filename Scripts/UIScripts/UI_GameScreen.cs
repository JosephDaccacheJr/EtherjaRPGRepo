using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class UI_GameScreen : MonoBehaviour
{
    public static UI_GameScreen instance;

    [Header("Screens")]
    public GameObject gameScreen;
    public GameObject characterSheetScreen, inventoryScreen, settingsScreen, gameOverScreen;

    [Header("Other UI Elements")]
    public GameObject loadingPopup;
    public GameObject noticeStatPoints;
    public TMP_Text textMessagePopup;
    public Image imageBlackScreen;
    Color _textMessageColor;
    float _textMessageTimer;
    bool _fadeScreen = false;

    private void Awake()
    {
        imageBlackScreen.color = new Color(0f, 0f, 0f, 1f);
        instance = this;
    }

    private void Start()
    {
        _textMessageColor = textMessagePopup.color;
        //ShowGameButtons();
    }

    public void GoBackToGameScreen()
    {
        characterSheetScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        settingsScreen.SetActive(false);
        ShowGameButtons();
    }

    public void ShowGameButtons()
    {
        gameScreen.SetActive(true);
        GameManager.instance.blockMovement = false;
        noticeStatPoints.SetActive(PlayerStats.instance.statPoints > 0);
    }

    public void HideGameButtons()
    {
        gameScreen.SetActive(false);
    }

    public void ShowCharacterSheet()
    {
        if (!GameManager.instance.IsPlayerAtDestination) return;
        GameManager.instance.blockMovement = true;
        characterSheetScreen.SetActive(true);
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        SoundManager.PlaySound(SoundManager.instance.uiOpen);
        HideGameButtons();
    }

    public void ShowInventorySreen()
    {
        if (!GameManager.instance.IsPlayerAtDestination) return;
        GameManager.instance.blockMovement = true;
        inventoryScreen.SetActive(true);
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        SoundManager.PlaySound(SoundManager.instance.uiOpen);
        HideGameButtons();
    }

    public void ShowSettingsScreen()
    {
        if (!GameManager.instance.IsPlayerAtDestination) return;
        GameManager.instance.blockMovement = true;
        settingsScreen.SetActive(true);
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        SoundManager.PlaySound(SoundManager.instance.uiOpen);
        HideGameButtons();
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        HideGameButtons();
    }

    public void ShowPopupMessage(string message)
    {
        _textMessageTimer = 5f;
        textMessagePopup.text = message;
    }

    void PopupMessageTimer()
    {
        if (_textMessageTimer > 0)
        {
            _textMessageTimer -= Time.unscaledDeltaTime;
            textMessagePopup.color = _textMessageColor;
        }
        else
        {
            textMessagePopup.color = new Color(_textMessageColor.r, _textMessageColor.g, _textMessageColor.b, Mathf.MoveTowards(textMessagePopup.color.a, 0f, Time.unscaledDeltaTime));
        }
    }

    void FadeScreenControl()
    {
        float alphaSet = _fadeScreen ? 1f : 0f;
        imageBlackScreen.color = new Color(imageBlackScreen.color.r, imageBlackScreen.color.g,
            imageBlackScreen.color.b, Mathf.MoveTowards(imageBlackScreen.color.a, alphaSet, Time.deltaTime));
    }

    public void SetFadeScreen(bool set)
    {
        _fadeScreen = set;
    }

    private void Update()
    {
        FadeScreenControl();
        PopupMessageTimer();
    }
}
