using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Create Item", order = 1)]
public class itemDataBase : ScriptableObject
{
    public string itemName;
    public int biometalCost;
    public int statToScaleCostWith; // If 0 ignore this
    public itemType type;
    public bool canUseOutOfBattle;
}
