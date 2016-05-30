using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Tooltip("Whether or not the MainMenu should be loaded.")]
    public bool SwitchToMainMenu = false;
    [Tooltip("Index of the scene to load. Only used when SwitchToMainMenu is set to false.")]
    public int sceneIndex = 0;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            if (SwitchToMainMenu)
                UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
