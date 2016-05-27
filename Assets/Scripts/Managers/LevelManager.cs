using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    void Awake()
    {
        // Set Players
        GameInfo.SetPlayersInGame();
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("CALLED");
        if(level > 1)
        {
            // Set Players
            GameInfo.SetPlayersInGame();
        }
    }
}

public static class GameInfo
{
    public static GameObject[] playersInGame;
    public static int mainMenuIndex = 1;

    public static void SetPlayersInGame()
    {
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
    }
}
