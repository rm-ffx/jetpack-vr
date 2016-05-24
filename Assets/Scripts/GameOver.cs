using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Game Over");
        }
    }
}
