using UnityEngine;
using System.Collections;

public class SwitchToMainMenu : MonoBehaviour
{
	void Start ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
