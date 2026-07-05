using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UI_InventoryItem : MonoBehaviour
{
    public UI_InventoryScreen invScreen;
    public TMP_Text textItemName;
    public TMP_Text textItemCount;
    public TMP_Text textItemCost;
    public Button buttonCraft;
    public Button buttonUse;

    itemDataBase _itemData;
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClickOnUse()
    {

    }

    public void ClickOnCraft()
    {
        int biometalCost = _itemData.biometalCost;
        switch (_itemData.type){
            case itemType.gunupgrade:
                biometalCost *= GameManager.instance.playerStats.gunLVL; break;
        }

        if (GameManager.instance.playerStats.bioMetal >= biometalCost)
        {
            GameManager.instance.playerStats.bioMetal -= biometalCost;
            GameManager.instance.CraftItem(_itemData.type);
            SoundManager.PlaySound(SoundManager.instance.uiUpgrade);
            invScreen.UpdateInventory();
            UpdateInventoryInfo();
            return;
        }
        else
        {

            SoundManager.PlaySound(SoundManager.instance.uiError);
        }
    }

    public void SetItemData(itemDataBase newData)
    {
        _itemData = newData;
        textItemName.text = _itemData.itemName;
        switch(_itemData.type) 
        {
            case itemType.medkit:
                textItemCount.text = GameManager.instance.playerStats.medkits.ToString();
                buttonUse.onClick.AddListener(GameManager.instance.UseMedikit);
                buttonUse.onClick.AddListener(UpdateInventoryInfo);
                textItemCost.text = string.Format("Costs {0} biometal", _itemData.biometalCost.ToString());
                break;
            case itemType.gunupgrade:
                textItemCount.text = GameManager.instance.playerStats.gunLVL.ToString();
                buttonUse.image.sprite = Resources.Load<Sprite>("Textures/UI/Rounded UI/UIButtonDefault");
                buttonUse.enabled = false;
                textItemCost.text = string.Format("Costs {0} biometal", Data.gunLvl[GameManager.instance.playerStats.gunLVL-1]);
                break;
        }

    }

    public void UpdateInventoryInfo()
    {
        switch (_itemData.type)
        {
            case itemType.medkit:
                textItemCount.text = GameManager.instance.playerStats.medkits.ToString();
                break;
            case itemType.gunupgrade:
                if (GameManager.instance.playerStats.gunLVL == Data.gunLvl.Count + 1)
                {
                    buttonCraft.enabled = false;
                    textItemCount.text = GameManager.instance.playerStats.gunLVL.ToString();
                    textItemCost.text = "MAX GUN LVL";

                }
                else
                {
                    buttonCraft.enabled = true;
                    textItemCount.text = GameManager.instance.playerStats.gunLVL.ToString();
                    textItemCost.text = string.Format("Costs {0} biometal", Data.gunLvl[GameManager.instance.playerStats.gunLVL - 1]);
                }
                break;
        }
        invScreen.UpdateInventory();
    }
}

