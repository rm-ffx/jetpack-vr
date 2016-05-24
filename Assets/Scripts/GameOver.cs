using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.name == "[CameraRig]")
        {
            Debug.Log("Game Over");
        }
    }
}
