using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;

    [Header("=== SOUNDS ===")]
    [Header("Common sounds")]
    public GameObject movePlayer;
    [Header("UISounds")]
    public GameObject uiBeep;
    public GameObject uiOpen, uiClose, uiClick,uiError,uiUpgrade,uiSpendPoint;
    [Header("Attack Sounds")]
    public GameObject playerAttackSingle;
    public GameObject playerAttackBurst;


    private void Awake()
    {
        if(SoundManager.instance == null) 
        {
            instance = this;
        }
    }

    public static void PlaySound(GameObject sound)
    {
        // TODO This is silly, using the instance is the instance itself, find an alternative
        Destroy(Instantiate(sound, SoundManager.instance.transform), 10f);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
