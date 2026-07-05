using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "BattleScriptableObjects/CreateNewEnemy", order = 1)]

public class EnemyData : ScriptableObject
{
    public string m_uniqueID;
    private System.Guid m_guid;
    public System.Guid GUID
    {
        get
        {
            if (m_guid == System.Guid.Empty)
                m_guid = System.Guid.Parse(m_uniqueID);
            return m_guid;
        }
    }

    public string m_name;
    public string m_graphic;
    public int m_level;
    public bool m_cannotFlee;
    [Header("Battle Stats")]
    public int HP;
    public int AC;
    public int expReward;
    public int attackModifier = 1;
    [Header("Battle Behavior")]
    [Range(0,1)]
    public float recklessness;
    [Range(0, 1)]
    public float cautiousness;
    

    public List<BattleMove> battleMovesRegular = new List<BattleMove>();
    public List<BattleMove> battleMovesCautious = new List<BattleMove>();
    public List<BattleMove> battleMovesReckless = new List<BattleMove>();


    [Header("Other Settigns")]
    public string onDefeatLoadScene;
}
