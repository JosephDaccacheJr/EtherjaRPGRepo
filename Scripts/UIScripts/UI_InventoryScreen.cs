using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class UI_InventoryScreen : MonoBehaviour
{
    public GameObject buttonCraftMedkit;
    public GameObject buttonUpgadeGun;
    public TMP_Text textMedikits;
    public TMP_Text textGunLVL;
    public TMP_Text textBiometal;
    public TMP_Text textHP;
    PlayerStats _playerStats;

    public GameObject itemContainer;
    public GameObject itemPrefab;

    List<itemDataBase> _items = new List<itemDataBase>();
    List<GameObject> _itemObjects = new List<GameObject>();

    private void Start()
    {
    }

    private void OnEnable()
    {
        if (_items.Count == 0)
        {
            PopulateItemList();
        }
        _playerStats = PlayerStats.instance;
        UpdateInventory();
    }
    
    public void PopulateItemList()
    {
        foreach(GameObject i in _itemObjects)
        {
            Destroy(i);
        }

        _itemObjects.Clear();
        _items.Clear();

        foreach(itemDataBase item in Resources.LoadAll("Prefabs/Items"))
        {
            _items.Add(item);
        }

        foreach (itemDataBase item in _items)
        {
            GameObject newItem = Instantiate(itemPrefab);
            newItem.transform.parent = itemContainer.transform;
            newItem.transform.localScale = Vector3.one;
            UI_InventoryItem itemScript = newItem.GetComponent<UI_InventoryItem>();
            itemScript.SetItemData(item);
            itemScript.invScreen = this;
        }
    }


    public void UpdateInventory()
    {
        textMedikits.text = _playerStats.medkits.ToString();
        textGunLVL.text = _playerStats.gunLVL.ToString();
        textBiometal.text = _playerStats.bioMetal.ToString();
        textHP.text = _playerStats.HP.ToString() + "/" + _playerStats.GetMaxHP().ToString();
        buttonCraftMedkit.SetActive(_playerStats.bioMetal >= 5);
        buttonUpgadeGun.SetActive(GameManager.instance.playerStats.gunLVL < Data.gunLvl.Count-1 && _playerStats.bioMetal >= Data.gunLvl[GameManager.instance.playerStats.gunLVL-1]);

    }

    public void ClickedOnClose()
    {
        gameObject.SetActive(false);
        SoundManager.PlaySound(SoundManager.instance.uiClose);
        UI_GameScreen.instance.ShowGameButtons();
    }
}
