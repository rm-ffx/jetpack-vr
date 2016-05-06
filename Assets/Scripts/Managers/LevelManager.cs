using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager:MonoBehaviour
{
    void Awake()
    {
        // Set Players
        LevelInfo.SetPlayersInGame();

        // Set Waypoints
        LevelInfo.SetWaypoints();
    }
}

public static class LevelInfo
{
    public static GameObject[] playersInGame;
    public static List<Transform> npcPatrolWaypoints;

    public static void SetPlayersInGame()
    {
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
    }

    public static void SetWaypoints()
    {
        npcPatrolWaypoints = new List<Transform>();
        GameObject[] foundWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        for (int i = 0; i < foundWaypoints.Length; i++) npcPatrolWaypoints.Add(foundWaypoints[i].transform);
    }
}
