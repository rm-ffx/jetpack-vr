using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
