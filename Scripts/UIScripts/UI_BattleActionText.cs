using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BattleActionText : MonoBehaviour
{
    public TMP_Text textBattleMessage;
    RectTransform rectTrans;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    void Update()
    {
        Color textColor = textBattleMessage.color;
        textBattleMessage.color = new Color(textColor.r, textColor.g, textColor.b, Mathf.MoveTowards(textColor.a, 0f, Time.deltaTime * 1f));
        rectTrans.anchoredPosition += new Vector2(0f, -40f) * Time.deltaTime;
    }
}
