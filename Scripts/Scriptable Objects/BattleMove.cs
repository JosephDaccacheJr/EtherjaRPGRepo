using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "BattleScriptableObjects/CreateNewMove", order = 2)]

public class BattleMove : ScriptableObject
{
    public enum moveType { regular, cautious, reckless };
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
    public moveType m_moveType;
    public bool healMessage;
    public int hpChangePlayer;
    public int hpChangeSelf;
    [Range(0,1)]
    public float accuracy;
    public int accuracyBoost;
    public AudioClip attackSound;
    public bool playSoundEvenIfMiss;
}
