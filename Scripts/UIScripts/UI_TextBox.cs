using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_TextBox : MonoBehaviour
{
    public RectTransform textRect;
    public TMP_Text textMessage;
    public Action<string> clickOnAction;
    public string request;

    public void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 10f);
    }

    private void OnEnable()
    {
    }

    public void ShowMessage(int msgID, Action<string> newAction = null)
    {
        clickOnAction = newAction;
        gameObject.SetActive(true);
        textMessage.text = Data.messages[msgID];
        textRect.anchoredPosition = Vector3.zero;
        transform.localScale = new Vector3(1f, 0f, 1f);
    }

    public void ClickedOnClose()
    {
        if(clickOnAction != null)
            clickOnAction(request);

        gameObject.SetActive(false);
    }
}
