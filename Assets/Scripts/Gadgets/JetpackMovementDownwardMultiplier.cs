using UnityEngine;
using System.Collections;

public class JetpackMovementDownwardMultiplier : MonoBehaviour
{
    [Tooltip("The Jetpack Movement Script of the left controller.")]
    public JetpackMovement LeftHandJM;
    [Tooltip("The Jetpack Movement Script of the right controller.")]
    public JetpackMovement RightHandJM;
    [Tooltip("Multiplier to control downward speed in order to make falling down more realistic.")]
    public float downwardMultiplier = 1.0f;

    private Rigidbody m_rigidBody;

	// Use this for initialization
	void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(LeftHandJM.enabled == false && RightHandJM.enabled == false)
        {
            m_rigidBody.AddForce(Vector3.down * 0.5f * downwardMultiplier, ForceMode.VelocityChange);
        }
    }
}
