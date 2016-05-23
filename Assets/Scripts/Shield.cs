using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    public bool looseEnergyOverTime = false;
    public bool looseEnergyOnHit = false;
    public bool startWithFullEnergy = true;
    public float maxEnergy = 100.0f;
    public float energyRestoration = 1.0f;

    public GameObject shieldObject;

    private float m_actualEnergy;

	// Use this for initialization
	void Start ()
    {
        if(startWithFullEnergy)
            m_actualEnergy = maxEnergy;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (looseEnergyOverTime)
            m_actualEnergy -= Time.deltaTime;
	}
}
