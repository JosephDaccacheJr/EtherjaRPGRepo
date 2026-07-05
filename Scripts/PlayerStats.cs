using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int perception, agility, endurance;
    public int lvl;
    public int exp;
    public int HP;
    public int medkits; // In the future this will be full fledged inventory
    public int gunLVL;
    public int bioMetal;
    public int statPoints;

    // Quest Variables
    public List<int> readMessages = new List<int>();
    public List<int> seenStories = new List<int>();
    public List<int> variablesSet = new List<int>();
    public List<int> mapItemVariables = new List<int>();

    public static PlayerStats instance;

    public int playerPositionX, playerPositionY;

    public int GetAC()
    {
        return (lvl * 2) + agility;
    }

    public int GetMaxHP()
    {
        return (lvl * 5) + endurance;
    }
    
    public void SetStartingStats()
    {
        lvl = 1;
        exp = 0;
        perception = 5;
        agility = 5;
        endurance = 5;
        medkits = 3;
        bioMetal = 0;
        statPoints = 0;
        gunLVL = 1;
        HP = GetMaxHP();

        readMessages = new List<int>();
        seenStories = new List<int>();
        variablesSet = new List<int>();
        mapItemVariables = new List<int>();

    }
}
