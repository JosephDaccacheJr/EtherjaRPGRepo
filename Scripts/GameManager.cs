using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    // TODO 
    // Medkit only heals 2 on map but 6 in battle
    public enum gammod
    {
        exploring, message, battle
    }
    public gammod gameMode;
    public static GameManager instance;

    public Dictionary<Vector2, MapTileData> mapData = new Dictionary<Vector2, MapTileData>();

    public BattleManager battleMan;

    public PlayerStats playerStats = new PlayerStats();

    [Header("World References")]
    public GameObject playerTile;
    Map_Player _player;

    [Header("UI References")]
    public UI_TextBox uiTextBox;
    public UI_StoryScreen uiStoryScreen;
    public bool blockMovement;
    public GameObject textCheatsEnabled;

    [Header("Map Tiles")]
    public GameObject gridBasicGreen;
    public GameObject gridBasicRed;
    public GameObject gridAlienGround01;



    [Header("Game Items")]
    public List<itemDataBase> items = new List<itemDataBase>(); // Making this scriptable so I can have this practice in the game too

    [Header("CHEATS")]
    public bool disableEncounters;
    public bool superPowered;
    public bool instantHeal;

    // Player Variables
    public bool IsPlayerAtDestination
    {
        get { return _player.IsAtDestination(); }
        set { }
    }

    // Save System
    SaveSystem saveSystem;



    // Random Encounter Controls
    public int safeSteps
    {
        get { return _safeSteps; }
        private set { _safeSteps = value; }
    }
    public int encounterZoneID
    {
        get { return _encounterZoneID; }
        private set { _encounterZoneID = value; }
    }
    public float randomEncounterChance
    {
        get { return _randomEncounterChance; }
        private set { _randomEncounterChance = value; }
    }

    int _safeSteps = 3;
    int _encounterZoneID = 0;
    float _randomEncounterChance = 0.3f;
    public List<EnemyData> enemies = new List<EnemyData>();

    private void Awake()
    {
        if (GameManager.instance == null)
            instance = this;
        else
            Destroy(gameObject);

        saveSystem = GetComponent<SaveSystem>();
    }


    void Start()
    {
        Debug.Log("GM Start");
        playerStats.SetStartingStats();
        PlayerStats.instance = playerStats;
        BuildMap();
        UI_GameScreen.instance.ShowGameButtons();
        _player = playerTile.GetComponent<Map_Player>();
        Application.targetFrameRate = 60;
        foreach(EnemyData enemy in Resources.LoadAll("Prefabs/Enemies"))
        {
            enemies.Add(enemy);
        }

        foreach(itemDataBase item in Resources.LoadAll("Prefabs/Items"))
        {
            items.Add(item);
        }
        
        MusicManager.instance.PlayExplore();
        textCheatsEnabled.SetActive(disableEncounters || superPowered || instantHeal);
    }
    
    void Update()
    {
        switch(gameMode)
        {
            case gammod.exploring:
                if(blockMovement)
                {
                    break;
                }
                if (Input.GetButtonDown("Horizontal"))
                {
                    Vector3 mov = new Vector3(Input.GetAxis("Horizontal") > 0 ? 1f : -1f, 0f, 0f);
                    if (_player.IsAtDestination() && IsMoveValid(mov))
                        MovePlayer(mov);
                }
                else if (Input.GetButtonDown("Vertical"))
                {
                    Vector3 mov = new Vector3(0f, Input.GetAxis("Vertical") > 0 ? 1f : -1f, 0f);
                    if (_player.IsAtDestination() && IsMoveValid(mov))
                        MovePlayer(mov);

                }
                break;
            case gammod.message:
                break;
        }
        Cheats();

    }

    void Cheats()
    {
        if (instantHeal  && Input.GetKeyDown(KeyCode.H))
        {
            playerStats.HP = playerStats.GetMaxHP();
        }
    }

    public void BuildMap()
    {
        mapData.Clear();
        foreach (GameObject grid in GameObject.FindGameObjectsWithTag("MapGrid"))
        {
            mapData.Add(grid.transform.position, grid.GetComponent<MapTileData>());
            grid.GetComponent<MapTileData>().UpdateMapGraphic();
        }
    }

    public void SetEncounterTable(List<EnemyData> newEnemies, int encZoneID)
    {
        encounterZoneID = encZoneID;
        enemies = newEnemies;
    }

    public bool IsMoveValid(Vector3 moveDir)
    {
        Vector2 playerPos = new Vector2(_player.transform.position.x, _player.transform.position.y);
        Vector2 moveTo = (Vector2)moveDir + playerPos;

        if (!mapData.ContainsKey(moveTo) || !mapData[moveTo].CanIWalk()) return false;

        return true;
    }

    public void PlayerFinishedMove(Vector3 pos)
    {
        Vector2 gridPos = new Vector2(pos.x, pos.y);
        if (!PlayerStats.instance.variablesSet.Contains(mapData[gridPos].giveVariable))
            PlayerStats.instance.variablesSet.Add(mapData[gridPos].giveVariable);

        switch (mapData[gridPos].tileType)
        {
            case MapTileData.type.message:  
                if(!PlayerStats.instance.readMessages.Contains(mapData[gridPos].messageID) || mapData[gridPos].repeatMessage)
                {
                    gameMode = gammod.message;
                    if (!PlayerStats.instance.readMessages.Contains(mapData[gridPos].messageID)) PlayerStats.instance.readMessages.Add(mapData[gridPos].messageID);
                    uiTextBox.ShowMessage(mapData[gridPos].messageID, DefaultCloseMessage);
                }
                break;
            case MapTileData.type.story:
                if (!PlayerStats.instance.seenStories.Contains(mapData[gridPos].storyID))
                {
                    gameMode = gammod.message;
                    PlayerStats.instance.seenStories.Add(mapData[gridPos].storyID);
                    uiStoryScreen.StartStory(mapData[gridPos].storyID, DefaultCloseMessage);
                    uiStoryScreen.gameObject.SetActive(true);
                }
                break;
            case MapTileData.type.startBattle:
                battleMan.StartBattle(mapData[gridPos].enemy);
                battleMan.sceneChange = mapData[gridPos].sceneChange;
                break;

            case MapTileData.type.biometal:
                if (PlayerStats.instance.mapItemVariables.Contains(mapData[gridPos].itemID)){ break; }
                PlayerStats.instance.bioMetal += mapData[gridPos].bioMetal;
                UI_GameScreen.instance.ShowPopupMessage(("Obtained " + mapData[gridPos].bioMetal.ToString() + " biometal"));
                PlayerStats.instance.mapItemVariables.Add(mapData[gridPos].itemID);
                mapData[gridPos].UpdateMapGraphic();
                break;
            default:
                if (!mapData[gridPos].noRandomEncounter)
                    RollRandomEncounter();
                break;
        }
    }

    public void RollRandomEncounter()
    {
#if UNITY_EDITOR
        if (disableEncounters) return;
#endif

        if(_safeSteps > 0)
        {
            _safeSteps--;
        }
        else if(enemies.Count > 0 && Random.Range(0f, 1f) <= _randomEncounterChance)
        {
            _safeSteps = (int)Random.Range(3, 5);
            battleMan.StartBattle(enemies[(int)Random.Range(0, enemies.Count)]);
        }
    }

    public void DefaultCloseMessage(string request)
    {
        gameMode = gammod.exploring;
    }

    public void MovePlayer(Vector3 mov)
    {
        _player.moveDestination += mov;
        SoundManager.PlaySound(SoundManager.instance.movePlayer);
    }

    public void InitiateBattle(int battleID)
    {
        battleMan.StartBattle(enemies[battleID]);
    }

    public void SaveGame()
    {
        saveSystem.SaveGame(playerTile);

        UI_GameScreen.instance.GoBackToGameScreen();
    }

    public void LoadGame()
    {
        UI_GameScreen.instance.loadingPopup.SetActive(true);
        SaveData SaveData;
        SaveData = saveSystem.LoadGameData();
        playerStats = SaveData.playerStats;
        PlayerStats.instance = playerStats;
        randomEncounterChance = SaveData.encounterRate;
        safeSteps = SaveData.safeSteps;

        _player.moveDestination = new Vector3(playerStats.playerPositionX, playerStats.playerPositionY, -1f);
        playerTile.transform.localPosition = new Vector3(playerStats.playerPositionX, playerStats.playerPositionY, -1f);

        foreach (GameObject t in GameObject.FindGameObjectsWithTag("EncounterZone"))
        {
            EncounterZone encZone = t.GetOrAddComponent<EncounterZone>();
            if (encZone.encounterZoneID == encounterZoneID)
            {
                SetEncounterTable(encZone.enemies, encZone.encounterZoneID);
                break;
            }
        }
        BuildMap();
        UI_GameScreen.instance.GoBackToGameScreen();
        UI_GameScreen.instance.loadingPopup.SetActive(false);
    }

    #region Player Actions
    public void UseMedikit()
    {
        
        if (playerStats.medkits <= 0 || playerStats.HP == playerStats.GetMaxHP()) return;
        playerStats.medkits--;
        SoundManager.PlaySound(SoundManager.instance.heal);
        float maxHP = playerStats.GetMaxHP();
        // TODO make the healing calculation set at a singlular place that can be referenced
        // anywhere
        float playerHP = Mathf.Clamp(playerStats.HP + (maxHP * 0.6f),
                                     0f, maxHP);
        playerStats.HP = (int)playerHP;

    }

    public void CraftItem(itemType type)
    {
        switch(type) 
        {
            case itemType.medkit:
                playerStats.medkits++;
                break;
            case itemType.gunupgrade:
                playerStats.gunLVL++;
                break;
        }
    }
    #endregion
}


public struct SaveData
{
    public Vector2 playerPosition;
    public int safeSteps;
    public int encounterTable;
    public float encounterRate;
    public PlayerStats playerStats;
}

public enum itemType { medkit, gunupgrade }
