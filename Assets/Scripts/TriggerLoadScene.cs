using UnityEngine;
using System.Collections;

public class TriggerLoadScene : TriggerScript
{
    [Tooltip("Wheter or not the MainMenu should be loaded.")]
    public bool SwitchToMainMenu = false;
    [Tooltip("Index of the scene to load. Only used when SwitchToMainMenu is set to false.")]
    public int sceneIndex = 0;

    public override void Activate()
    {
        if(SwitchToMainMenu)
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public override void Deactivate()
    {
        // No need to do anything here, since the scene will be changed upon activation
    }
}
