using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    private void Awake()
    {
        if(SceneController.instance == null)
        { SceneController.instance = this; }
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Cutscene_Intro");
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncload = SceneManager.LoadSceneAsync("Game");

        while(!asyncload.isDone) 
        {
            yield return null;
        }
        GameManager.instance.LoadGame();
    }
}
