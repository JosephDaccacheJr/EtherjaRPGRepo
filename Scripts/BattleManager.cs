using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    enum turn { player, enemy, playerToEnemy, enemyToPlayer, playerWins };
    turn currentTurn;


    // Battle Data
    public EnemyData currentEnemy;
    public List<Action<object>> battleActions = new List<Action<object>>();
    List<object> actionParameters = new List<object>();
    public string sceneChange;

    bool _isRunningActions;
    int _currentEnemyHP;

    float _playerAimMod;
    float _playerDMGMod;


    int _resultMsgIndex;

    List<String> _endOfBattleMessages = new List<string>();

    [Header("UI Elements")]
    public GameObject setAttackButtons;
    public GameObject setBattleResult;
    public GameObject battleViewer;
    public GameObject enemyScreen;
    public Image enemyImage;
    public Animator enemyAnim;
    public TMP_Text textEnemyHP;
    public TMP_Text textPlayerHP;
    public TMP_Text textBattleResult;
    public TMP_Text textMedkitCount;
    public Button buttonAttack, buttonAttackBurst, buttonUseMedkit, buttonFlee;
    List<GameObject> _BattleActionTexts = new List<GameObject>(); // To ensure no messages linger

    [Header("Prefabs")]
    public GameObject actionText;
    void Start()
    {

    }

    private void Update()
    {
        DebugCalls();
    }

    public void StartBattle(EnemyData newEnemy)
    {
        _endOfBattleMessages.Clear();
        UI_GameScreen.instance.HideGameButtons();
        GameManager.instance.gameMode = GameManager.gammod.battle;
        MusicManager.instance.PlayBattle();
        Debug.Log("<color=yellow>BATTLE: Start Battle</color>");
        battleActions.Clear();
        _isRunningActions = false;

        setAttackButtons.SetActive(true);
        setBattleResult.SetActive(false);

        textBattleResult.text = "";
        currentEnemy = newEnemy;
        enemyImage.sprite = Resources.Load<Sprite>("Textures/EnemyGraphics/" + newEnemy.m_graphic);

        enemyAnim.Play("Enemy_Idle");
        _currentEnemyHP = currentEnemy.HP;
        UpdateInfo();
        SetBattleControls(false);
        gameObject.SetActive(true);
        AddAction(StartPlayerTurn, "");
        TryToStartRunningActions();
    }

    IEnumerator DisplayBattleResult()
    {
        textBattleResult.text += _endOfBattleMessages[_resultMsgIndex];
        yield return new WaitForSeconds(1);
        _resultMsgIndex++;
        if (_resultMsgIndex < _endOfBattleMessages.Count)
            StartCoroutine(DisplayBattleResult());

    }

    public void BattleResult(bool playerWon)
    {
        if(sceneChange != "")
        {
            SceneManager.LoadScene(sceneChange);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void TryToStartRunningActions()
    {
        if (!_isRunningActions)
        {
            _isRunningActions = true;
            // 0.5f Min
            StartCoroutine(StartActions(1f));
        }
    }

    public IEnumerator StartActions(float waitTime)
    {

        battleActions[0].Invoke(actionParameters[0]);
        float waitTimer = (battleActions[0] == StartPlayerTurn) ? 0 : waitTime;
        battleActions.RemoveAt(0);
        actionParameters.RemoveAt(0);
        while (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            yield return null;
        }
        if (battleActions.Count > 0)
        {
            StartCoroutine(StartActions(waitTime));
        }
        else
        {
            _isRunningActions = false;
        }
    }

    public void PlayerAttack(object variables)
    {
        Debug.Log("<color=yellow>BATTLE: PlayerAttack</color>");
        float hitDice = ((UnityEngine.Random.Range(0, 20) + 1) + PlayerStats.instance.perception) * _playerAimMod;
        Debug.Log("<color=yellow>BATTLE: Rolled " + hitDice + " against " + currentEnemy.AC + "</color>");
        // TODO: vvv Something causes this to say cast is not valid. Maybe just from debug instant wins?
        if ((bool)variables == true)
            SoundManager.PlaySound(SoundManager.instance.playerAttackBurst);
        else
            SoundManager.PlaySound(SoundManager.instance.playerAttackSingle);


        if (hitDice > currentEnemy.AC)
        {

            int dmg = ((int)UnityEngine.Random.Range(0, 4) + 1) * (int)_playerDMGMod;
            _currentEnemyHP -= dmg;
            UpdateInfo();
            enemyAnim.Play("Enemy_Hit");
            ShowActionText("HIT!\n" + dmg);
            Debug.Log("<color=yellow>BATTLE: HIT enemy</color>");
        }
        else
        {
            ShowActionText("MISS!");
            Debug.Log("<color=yellow>BATTLE: MISSED enemy</color>");
        }
        CheckEnemyStatus("");
    }

    public void PlayerUseMedkit(object variables)
    {
        GameManager.instance.UseMedikit();

        ShowActionText("HEALING");
        UpdateInfo();
        AddAction(StartEnemyTurn, "");
    }

    void PlayerAttemptFlee(object variables)
    {
        AddAction(PlayerCalculateFlee, "");
    }

    void PlayerCalculateFlee(object variables)
    {
        float fleeDice = UnityEngine.Random.Range(0, 20) + PlayerStats.instance.agility;
        if (fleeDice >= currentEnemy.m_level * 5)
        {
            CloseBattleWindow();
            return;
        }

        ShowActionText("FAILED!");

        AddAction(StartEnemyTurn, "");
        UpdateInfo();
    }

    public void AnnounceTurn(object variables)
    {
        Debug.Log("BATTLE: TURN: PLAYER!");
    }

    public void StartPlayerTurn(object variables)
    {
        currentTurn = turn.player;
        SetBattleControls(true);
        Debug.Log("<color=yellow>BATTLE: Start player turn</color>");
    }

    public void StartEnemyTurn(object variables)
    {
        currentTurn = turn.enemy;
        SetBattleControls(false);
        Debug.Log("<color=yellow>BATTLE: Start enemy turn</color>");
        AddAction(DoEnemyMove, "");
    }

    public void DoEnemyMove(object variables)
    {
        BattleMove chosenMove = new BattleMove();
        float chooseMoveTypeRange = 1f;

        chooseMoveTypeRange += currentEnemy.recklessness + currentEnemy.cautiousness;
        float moveDice = UnityEngine.Random.Range(0, chooseMoveTypeRange);
        if (moveDice <= currentEnemy.recklessness && currentEnemy.battleMovesReckless.Count != 0)
        {
            // Chose Reckless move
            chosenMove = currentEnemy.battleMovesReckless[(int)UnityEngine.Random.Range(0, currentEnemy.battleMovesRegular.Count)];
        }
        else if (moveDice > currentEnemy.recklessness && moveDice <= currentEnemy.cautiousness + currentEnemy.recklessness
            && currentEnemy.battleMovesCautious.Count != 0)
        {
            // Chose Cautious move
            chosenMove = currentEnemy.battleMovesCautious[(int)UnityEngine.Random.Range(0, currentEnemy.battleMovesRegular.Count)];
        }
        else
        {
            // Chose Regular move
            chosenMove = currentEnemy.battleMovesRegular[(int)UnityEngine.Random.Range(0, currentEnemy.battleMovesRegular.Count)];
        }


        switch (chosenMove.healMessage)
        {
            case true:
                Debug.Log("<color=yellow>BATTLE: Enemy Healing </color>");
                ShowActionText("HEALING!");
                _currentEnemyHP = Mathf.Clamp(_currentEnemyHP + chosenMove.hpChangeSelf, 0, currentEnemy.HP);
                break;
            case false:
                float hitDice = (UnityEngine.Random.Range(0f, 20f) * (chosenMove.accuracy))  + chosenMove.accuracyBoost;

                Debug.Log("<color=yellow>BATTLE: Rolled " + hitDice + " against " + PlayerStats.instance.GetAC() + "</color>");

                enemyAnim.Play("Enemy_Attack");
                if (hitDice >= PlayerStats.instance.GetAC())
                {
                    int damageToPlayer = (chosenMove.hpChangePlayer * currentEnemy.attackModifier) - (PlayerStats.instance.endurance / 2);
                    damageToPlayer = Mathf.Clamp(damageToPlayer, 0, int.MaxValue);
                    PlayerStats.instance.HP -= damageToPlayer;
                    ShowActionText("ENEMY HITS!\n" + damageToPlayer);
                    Debug.Log("<color=yellow>BATTLE: Player Hit!</color>");
                }
                else
                {
                    ShowActionText("ENEMY MISSES!");
                    Debug.Log("<color=yellow>BATTLE: Player Missed</color>");
                }

                _currentEnemyHP = Mathf.Clamp(_currentEnemyHP - chosenMove.hpChangeSelf, 0, currentEnemy.HP);
                break;
        }


        CheckPlayerStatus("");
    }

    public void CheckEnemyStatus(object variables)
    {
        if (_currentEnemyHP <= 0)
        {
            _resultMsgIndex = 0;
            textEnemyHP.text = "";
            Debug.Log("<color=yellow>BATTLE: ENEMY KILLED</color>");
            _endOfBattleMessages.Add("VICTORY!\n");
            _endOfBattleMessages.Add("+" + currentEnemy.expReward + " EXP\n");
            PlayerStats.instance.exp += currentEnemy.expReward;
            MusicManager.instance.PlayVictory();
            setAttackButtons.SetActive(false);
            setBattleResult.SetActive(true);
            enemyAnim.Play("Enemy_Death");
            CheckEXPResult();
            StartCoroutine(DisplayBattleResult());
            currentTurn = turn.playerWins;
        }
        else
        {
            AddAction(StartEnemyTurn, "");
        }
    }

    public void CheckPlayerStatus(object variables)
    {
        UpdateInfo();
        if (PlayerStats.instance.HP <= 0)
        {
            textPlayerHP.text = "DEAD";
            Debug.Log("<color=yellow>BATTLE: YOU DIED</color>");
            //CloseBattleWindow();
            //GameManager.instance.LoadGame();
            AddAction(DoGameOver, "");


        }
        else
        {
            AddAction(StartPlayerTurn, "");
        }
    }

    public void DoGameOver(object variables)
    {
        CloseBattleWindow();
        UI_GameScreen.instance.ShowGameOverScreen();
    }

    public void CheckEXPResult()
    {
        if (PlayerStats.instance.lvl < Data.lvl.Count + 1 && PlayerStats.instance.exp >= Data.lvl[PlayerStats.instance.lvl])
        {
            PlayerStats.instance.lvl++;
            PlayerStats.instance.statPoints++;

            _endOfBattleMessages.Add("LEVEL UP!\n+SKILL POINT");
        }
    }

    public void SetBattleControls(bool set)
    {
        buttonAttack.interactable = set;
        buttonAttackBurst.interactable = set;
        buttonUseMedkit.interactable = set;
        buttonFlee.interactable = currentEnemy.m_cannotFlee ? false : set;
    }

    public void UpdateInfo()
    {
        textEnemyHP.text = _currentEnemyHP.ToString("000") + "/" + currentEnemy.HP.ToString("000");
        textPlayerHP.text = PlayerStats.instance.HP.ToString("000") + "/" + PlayerStats.instance.GetMaxHP().ToString("000");
        textMedkitCount.text = PlayerStats.instance.medkits.ToString("000");
    }

    #region Button Clicks

    public void CloseBattleWindow()
    {
        foreach(GameObject actionText in _BattleActionTexts)
        {
            Destroy(actionText);
        }
        _BattleActionTexts.Clear();

        SoundManager.PlaySound(SoundManager.instance.uiClose);

        if (sceneChange != "")
        {
            MusicManager.instance.volumeBattle = 0f;
            MusicManager.instance.volumeVictory = 0f;
            UI_GameScreen.instance.SetFadeScreen(true);
            Invoke("LoadScene", 1f);
        }
        else
        {
            GameManager.instance.gameMode = GameManager.gammod.exploring;
            MusicManager.instance.PlayExplore();
            UI_GameScreen.instance.ShowGameButtons();
        }
        gameObject.SetActive(false);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneChange);
    }
    public void ClickOnAttack(bool isBurst)
    {
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        _playerAimMod = isBurst ? 0.35f : 1f;
        _playerDMGMod = isBurst ? 3f : 1f;
        _playerDMGMod = _playerDMGMod * (1 + ((PlayerStats.instance.gunLVL - 1)));
        if (currentTurn == turn.player)
        {
            object var = isBurst;
            currentTurn = turn.playerToEnemy;
            AddAction(PlayerAttack, isBurst);
        }
        SetBattleControls(false);
        TryToStartRunningActions();
    }

    public void ClickOnMedkit()
    {
        if (GameManager.instance.playerStats.medkits <= 0 ||
            GameManager.instance.playerStats.HP == GameManager.instance.playerStats.GetMaxHP())
        {

            SoundManager.PlaySound(SoundManager.instance.uiError);
            return;
        }


        SoundManager.PlaySound(SoundManager.instance.uiClick);
        AddAction(PlayerUseMedkit, "");
        SetBattleControls(false);
        TryToStartRunningActions();

    }

    public void ClickOnFlee()
    {
        SoundManager.PlaySound(SoundManager.instance.uiClick);
        ShowActionText("ATTEMPTING FLEE...");
        AddAction(PlayerAttemptFlee, "");
        SetBattleControls(false);
        TryToStartRunningActions();

    }
    #endregion

    public void ShowActionText(string text)
    {
        GameObject newAction = Instantiate(actionText, enemyScreen.transform);
        newAction.GetComponent<UI_BattleActionText>().textBattleMessage.text = text;
        _BattleActionTexts.Add(newAction);
        Destroy(newAction, 4f);
    }

    void AddAction(Action<object> action, object vars)
    {
        actionParameters.Add(vars);
        battleActions.Add(action);
    }

    void DebugCalls()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            battleActions.Clear();
            _currentEnemyHP = 0;
            CheckEnemyStatus("");
        }
    }
}
