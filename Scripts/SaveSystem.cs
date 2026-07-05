using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class SaveSystem : MonoBehaviour
{

    internal List<int> readMessages = new List<int>();
    internal List<int> variablesSet = new List<int>();
    internal List<int> mapItemVariables;
    internal Vector2 playerPosition;
    GameObject _player;

    public void SaveGame(GameObject player)
    {
        _player = player;
        GatherGameState();
    }

    public void GatherGameState()
    {
        PlayerStats.instance.playerPositionX = (int)_player.gameObject.transform.position.x;
        PlayerStats.instance.playerPositionY = (int)_player.gameObject.transform.position.y;

        WriteToFile();
    }

    void WriteToFile()
    {
        Debug.Log(JsonConvert.SerializeObject(PlayerStats.instance, Formatting.Indented));
        string savePath = Application.persistentDataPath + "/save.JSON";
        StreamWriter sw = new StreamWriter(savePath);
        sw.WriteLine(JsonConvert.SerializeObject(PlayerStats.instance, Formatting.Indented));
        sw.Close();
        Debug.Log("SAVED TO: " + savePath);
    }

    public SaveData LoadGameData()
    {
        SaveData SaveData = new SaveData();
        SaveData.playerStats = new PlayerStats();

        Debug.Log("LOADING DATA FROM '" + Application.persistentDataPath + "/save.JSON'");   
        string savePath = Application.persistentDataPath + "/save.JSON";
        StreamReader sr = new StreamReader(savePath);

        string saveFile = "";
        
        List<string> readLines = new List<string>();

        while(!sr.EndOfStream)
        {
            saveFile += sr.ReadLine();
        }
        SaveData.playerStats = JsonConvert.DeserializeObject<PlayerStats>(saveFile);
        SaveData.encounterRate = GameManager.instance.randomEncounterChance;
        return SaveData;
    }
}
