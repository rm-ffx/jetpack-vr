using UnityEngine;
using System.Collections.Generic;

public class SpawnPortal : MonoBehaviour
{
    public GameObject spawnObject;

    [Tooltip("Root Object that Contains the Waypoints that the NPCs spawned from this Portal will patrol.")]
    public GameObject patrolWaypoints;

    private List<Transform> waypoints = new List<Transform>(); // Waypoints between which the NPC will patrol

    void Awake()
    {
        // Cache Variables
        for (int i = 0; i < patrolWaypoints.transform.childCount; i++) waypoints.Add(patrolWaypoints.transform.GetChild(i));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject spaceMan = Instantiate(spawnObject, transform.position, transform.rotation) as GameObject;
            spaceMan.GetComponent<NPCInfo>().patrolWaypoints = waypoints;
        }
    }

}
