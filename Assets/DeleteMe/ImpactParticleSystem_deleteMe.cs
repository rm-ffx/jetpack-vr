using UnityEngine;
using System.Collections;

public class ImpactParticleSystem_deleteMe : MonoBehaviour
{
    [Tooltip("The lifetime of the particle system")]
    public float lifetime = 0.3f;
	
	// Update is called once per frame
	void Update ()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
            Destroy(gameObject);
	}
}
