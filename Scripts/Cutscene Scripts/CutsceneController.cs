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
    public int cutsceneDataStart;
    public int cutsceneIndex;
    public Image fadeOut;
    public string nextScene;
    public AudioSource cutsceneMusic;
    public AudioSource sfxClick;

    private void Awake()
    {
        fadeOut.color = new Color(0f, 0f, 0f, 1f);
    }
    void Start()
    {
        Debug.Log(cutsceneData.text);
        cutscenes = JsonConvert.DeserializeObject<List<CutsceneItem>>(cutsceneData.text);
        int cutsceneDataIndex = 0;
        foreach(var cutscene in cutsceneObjects) 
        {
            cutscene.GetComponentInChildren<TMP_Text>().text = cutscenes[cutsceneDataStart + cutsceneDataIndex].storyText[0];
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

        RunFadeOut();


    }

    void RunFadeOut()
    {
        float fadeTarget = cutsceneIndex < cutsceneObjects.Count ? 0f : 1f;
        fadeOut.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(fadeOut.color.a, fadeTarget, Time.deltaTime));
        cutsceneMusic.volume = 0.5f - (fadeOut.color.a / 2f);
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
        if(cutsceneIndex == cutsceneObjects.Count - 1 && cutsceneIndex < 99)
        {
            sfxClick.Play();
            Invoke("LoadNextScene", 1f);
            cutsceneIndex = 100;
        }
        else if(cutsceneIndex < cutsceneObjects.Count)
        {
            sfxClick.Play();
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