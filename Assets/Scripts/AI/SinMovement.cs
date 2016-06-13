using UnityEngine;
using System.Collections;

public class SinMovement : MonoBehaviour
{
    public float frequency = 20.0f;  // Speed of sine movement
    public float magnitude = 0.5f;   // Size of sine movement

    void Update()
    {
        transform.localPosition = Vector3.up * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
