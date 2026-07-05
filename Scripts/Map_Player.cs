using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Map_Player : MonoBehaviour
{
    public Vector3 moveDestination;
    int _lastEncounterZoneID;

    void Start()
    {
        moveDestination = transform.position;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!IsAtDestination())
        {
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * 6f);
            if(IsAtDestination())
            {
                GameManager.instance.PlayerFinishedMove(transform.position);
            }
        }
        
    }

    public bool IsAtDestination()
    {
        return transform.position == moveDestination;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered encounter zone " + collision.gameObject.name);
        if(collision.tag == "EncounterZone")
        {
            List<EnemyData> enemies = new List<EnemyData>();
            EncounterZone encZone = collision.gameObject.GetComponent<EncounterZone>();
            enemies = encZone.enemies;
            GameManager.instance.SetEncounterTable(enemies, encZone.encounterZoneID);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("I left " + collision.name);
    }

}
