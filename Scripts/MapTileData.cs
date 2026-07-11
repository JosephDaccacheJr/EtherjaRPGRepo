using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
[ExecuteInEditMode]
public class MapTileData : MonoBehaviour
{
 

    public enum type
    {
        normal,message,story, blockIfVariableNotFound, blockIfVariableFound,startBattle,biometal
    }

    [Header("Tile Graphics")]
    public GameObject biometalImage;


    [Header("Tile Settings")]
    public type tileType;
    public bool walkable;



    public bool noRandomEncounter;
    public int messageID;
    public int storyID;
    public EnemyData enemy;
    public string sceneChange;
    public bool repeatMessage;
    public List<int> variablesNeeded = new List<int>();
    public int giveVariable;
    public int tileID;
    
    public int itemID;
    public int bioMetal;

    [Header("Tile Markers")]
    public GameObject markerParent;
    public GameObject markerMessage, markerBattle, markerBlocker;

    private void Start()
    {
        markerParent.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        markerParent.SetActive(true);
    }

    public void RenameTileAndMarkTile(int newID)
    {
        tileID = newID;
        string xPos = (transform.position.x >= 0 ? " " : "") + transform.position.x.ToString("000");
        string yPos = (transform.position.y >= 0 ? " " : "") + transform.position.y.ToString("000");
        gameObject.name = "TILE_[" + xPos+ "," + yPos + "]_" + tileType + "_" + GetComponent<SpriteRenderer>().sprite.name;
        markerParent.SetActive(true);
        markerMessage.SetActive(false); markerBattle.SetActive(false) ; markerBlocker.SetActive(false) ;
        switch(tileType) 
        {
            case type.message:
                markerMessage.SetActive(true); break;
            case type.startBattle:
                markerBattle.SetActive(true); break;
            case type.story:
               // markerBattle.SetActive(true); break;
            case type.blockIfVariableFound:
            case type.blockIfVariableNotFound:
                markerBlocker.SetActive(true); break;
        }
    }

    public bool CanIWalk()
    {
        switch (tileType)
        {
            case type.blockIfVariableNotFound:
                foreach (int v in variablesNeeded)
                {
                    if (!PlayerStats.instance.variablesSet.Contains(v))
                        return false;
                }
                return true;

            case type.blockIfVariableFound:
                foreach (int v in variablesNeeded)
                {
                    if (PlayerStats.instance.variablesSet.Contains(v))
                        return false;
                }
                return true;
            default:
                return walkable;

        }
    }

    public void UpdateMapGraphic()
    {
        switch (tileType)
        {
            case type.biometal:
                biometalImage.SetActive(!PlayerStats.instance.mapItemVariables.Contains(itemID));
                break;
        }
    }


}   
