using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_StoryScreen : MonoBehaviour
{
    public RectTransform textRect;
    public TMP_Text textMessage;
    public Image imageStoryPic;
    public GameObject buttonNext, buttonClose;
    public Action<string> clickOnAction;
    public string request;
    StoryItem _currentStoryItem;
    int _currentStoryIndex;

    void Start()
    {
        //StartStory(1);
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 10f);
    }

    public void StartStory(int storyID, Action<string> newAction = null)
    {

        SoundManager.PlaySound(SoundManager.instance.uiOpen);
        transform.localScale = new Vector3(1f, 0f, 1f);
        clickOnAction = newAction;
        _currentStoryItem = Data.stories[storyID];
        _currentStoryIndex = 0;
        UpdateStory();
    }

    public void SetButtons()
    {
        buttonClose.SetActive(_currentStoryIndex == _currentStoryItem.storyText.Length - 1);
        buttonNext.SetActive(_currentStoryIndex != _currentStoryItem.storyText.Length - 1);
    }
    public void UpdateStory()
    {
        SetButtons();
        textRect.anchoredPosition = Vector3.zero;
        textMessage.text = _currentStoryItem.storyText[_currentStoryIndex];
        imageStoryPic.sprite = Resources.Load<Sprite>("Textures/UI/StoryImages/" + _currentStoryItem.storyImage[_currentStoryIndex]);
    }

    public void ClickNextPage()
    {
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        _currentStoryIndex++;
        UpdateStory();
    }
    public void ClickClose()
    {

        SoundManager.PlaySound(SoundManager.instance.uiClick);
        SoundManager.PlaySound(SoundManager.instance.uiClose);
        if (clickOnAction != null)
            clickOnAction(request);

        gameObject.SetActive(false);
    }
}
