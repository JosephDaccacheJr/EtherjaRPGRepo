using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_CharacterSheet : MonoBehaviour
{
    [Header("Player Stats")]
    public TMP_Text textLevel;
    public TMP_Text textHP;
    public TMP_Text textAC;
    public TMP_Text textStatPerception;
    public TMP_Text textStatAgility;
    public TMP_Text textStatEndurance;
    public TMP_Text textStatPoints;
    public TMP_Text textEXPPoints;

    [Header("Add Buttons")]
    public GameObject statAddPerception;
    public GameObject statAddAgility;
    public GameObject statAddEndurance;

    PlayerStats _playerStats;

    private void OnEnable()
    {
        _playerStats = PlayerStats.instance;
        UpdateSheet();
    }

    public void UpdateSheet()
    {
        textStatPerception.text = _playerStats.perception.ToString();
        textStatAgility.text = _playerStats.agility.ToString();
        textStatEndurance.text = _playerStats.endurance.ToString();
        textStatPoints.text = _playerStats.statPoints.ToString();
        textLevel.text = _playerStats.lvl.ToString();
        textHP.text = _playerStats.HP.ToString() + "/" + _playerStats.GetMaxHP().ToString();
        textAC.text = _playerStats.GetAC().ToString();
        if (_playerStats.lvl < Data.lvl.Count)
            textEXPPoints.text = _playerStats.exp.ToString("0000") + "/" + Data.lvl[_playerStats.lvl].ToString("0000");
        else
            textEXPPoints.text = "MAX LEVEL";

        bool isStatPoint = GameManager.instance.playerStats.statPoints > 0;
        statAddPerception.SetActive(isStatPoint);
        statAddAgility.SetActive(isStatPoint);
        statAddEndurance.SetActive(isStatPoint);
    }

    public void AddStatPoint(int statNum)
    {
        switch(statNum)
        {
            case 0:
                _playerStats.perception++;
                break;
            case 1:
                _playerStats.agility++;
                break;
            case 2:
                _playerStats.endurance++;
                break;
        }
        SoundManager.PlaySound(SoundManager.instance.uiSpendPoint);
        _playerStats.statPoints--;
        _playerStats.HP = _playerStats.GetMaxHP();
        UpdateSheet();
    }

    public void ClickedOnClose()
    {
        gameObject.SetActive(false);
        Destroy(Instantiate(SoundManager.instance.uiClose, SoundManager.instance.transform), 2f);
        UI_GameScreen.instance.ShowGameButtons();
    }

    public void ClickedOnHelp()
    {
        SoundManager.PlaySound(SoundManager.instance.uiOpen);
        Action<string> clickAction;
        clickAction = delegate (string s) { UI_GameScreen.instance.ShowCharacterSheet(); };
        GameManager.instance.uiTextBox.ShowMessage(8, clickAction);
        gameObject.SetActive(false);
    }
}
