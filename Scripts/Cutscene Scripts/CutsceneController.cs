using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{

    public List<CutsceneItem> cutscenes = new List<CutsceneItem>();

    public TextAsset cutsceneData;
    public List<Image> cutsceneImages;
    public List<TMP_Text> cutsceneTexts;
    public List<GameObject> cutsceneObjects;
    public int cutsceneIndex;
    public Image fadeOut;
    public string nextScene;
    void Start()
    {
        Debug.Log(cutsceneData.text);
        cutscenes = JsonConvert.DeserializeObject<List<CutsceneItem>>(cutsceneData.text);
        int cutsceneDataIndex = 0;
        foreach(var cutscene in cutsceneObjects) 
        {
            cutscene.GetComponentInChildren<TMP_Text>().text = cutscenes[cutsceneDataIndex].storyText[0];
            cutsceneDataIndex++;
        }

        for (int i = 0; i < cutsceneObjects.Count; i++)
        {
            float alphaTarget = i == cutsceneIndex ? 1f : 0f;

            foreach (Image img in cutsceneObjects[i].GetComponentsInChildren<Image>())
            {
                img.color = new Color(1f, 1f, 1f, 0f);
            }

            foreach (TMP_Text txt in cutsceneObjects[i].GetComponentsInChildren<TMP_Text>())
            {
                txt.color = new Color(1f, 1f, 1f,0f);
            }
        }
    }

    void Update()
    {
        if (cutsceneIndex < cutsceneObjects.Count)
            CutsceneDisplayControl();
        else
        {
            RunFadeOut();
        }

    }

    void RunFadeOut()
    {
        fadeOut.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(fadeOut.color.a, 1f, Time.deltaTime));
    }

    void CutsceneDisplayControl()
    {
        for(int i = 0; i < cutsceneObjects.Count; i++)
        {
            float alphaTarget = i == cutsceneIndex ? 1f : 0f;
            
            foreach(Image img in cutsceneObjects[i].GetComponentsInChildren<Image>())
            {
                img.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(img.color.a, alphaTarget, Time.deltaTime * 2f));
            }

            foreach (TMP_Text txt in cutsceneObjects[i].GetComponentsInChildren<TMP_Text>())
            {
                txt.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(txt.color.a, alphaTarget, Time.deltaTime * 2f));
            }
        }
    }

    public void ProgressCutscene()
    {
        if(cutsceneIndex >= cutsceneObjects.Count && cutsceneIndex < 99)
        {
            Invoke("LoadNextScene", 2f);
            cutsceneIndex = 100;
        }
        else if(cutsceneIndex < cutsceneObjects.Count)
        {
            cutsceneIndex++;
        }

    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
[System.Serializable]
public class CutsceneItem
{
    public string[] storyText;
}