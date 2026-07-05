using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.Mathematics;
using System.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class Data : MonoBehaviour
{
    // Some of these will use different means of data as a means of practicing
    // data management and retrieval.
    public static Data instance;
    public static List<Item_Weapon> weapons = new List<Item_Weapon>();
    public static List<string> messages = new List<string>();
    public static List<int> lvl = new List<int>();
    public static List<int> gunLvl = new List<int>();
    public static List<StoryItem> stories = new List<StoryItem>();
    public TextAsset storyData;
    public TextAsset LVLData;
    public TextAsset gunLVLData;
    public TextAsset messageData;

    private void Awake()
    {
        if (Data.instance == null)
        {
           // DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            Destroy(this);
    }

    void Start()
    {
        LoadStoryData();

        foreach (string newLine in messageData.text.Split('*'))
        {
            messages.Add(newLine.Split('#')[1]);
          //  messages[messages.Count - 1].Replace("\n", "\n");
        }

        lvl.Add(0);
        foreach(string newLine in LVLData.text.Split('\n'))
        { 
            lvl.Add(int.Parse(newLine));
        }

        foreach (string newLine in gunLVLData.text.Split('\n'))
        {
            gunLvl.Add(int.Parse(newLine));
        }

    }

    string ReadData(string fileName)
    {
        string path = Application.dataPath + "/data/" + fileName;
        #if UNITY_EDITOR
        path = "Assets/data/" + fileName;
        #endif
        StreamReader sr = new StreamReader(path);
        return  sr.ReadToEnd();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadStoryData()
    {
        stories = JsonConvert.DeserializeObject<List<StoryItem>>(storyData.text);
    }


}

[System.Serializable]
public class StoryItem
{
    public int ID;
    public string[] storyText;
    public string[] storyImage;

}
