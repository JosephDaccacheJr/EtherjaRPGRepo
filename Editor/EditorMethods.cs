using UnityEngine;
using UnityEditor;

public class EditorMethods : MonoBehaviour
{
    [MenuItem("Map/Build Tile Data")]
    static void BuildTileData()
    {
        // TODO
        // Make the HP instanttly heal on a level up and update it on the UI
        // Blue demon more XP?
        // The inventory screen doesn't update medikit count used in battle
        int tileID = 0;
        int itemID = 1;
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("MapGrid"))
        {
            MapTileData curTile = t.GetComponent<MapTileData>();
            curTile.RenameTileAndMarkTile(tileID);
            if (curTile.tileType == MapTileData.type.biometal)
            {
                curTile.itemID = itemID;
                itemID++;
            }
            tileID++;
        }
        int encounterZoneID = 0;
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("EncounterZone"))
        {
            t.GetComponent<EncounterZone>().encounterZoneID = encounterZoneID;
            encounterZoneID++;
        }

    }
}
