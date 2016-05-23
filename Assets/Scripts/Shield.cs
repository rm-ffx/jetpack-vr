using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Shield : MonoBehaviour
{
    public float maxEnergy = 100.0f;
    public bool startWithFullEnergy = true;
    public bool looseEnergyOnHit = false;
    public float looseEnergyOverTime = 0.0f;
    public float energyRestoration = 0.0f;

    public GameObject shieldObject;
    public Material shieldActiveMaterial;
    public Material shieldDeactivatedMaterial;

    private Collider m_shieldCollider;
    private MeshRenderer m_shieldRenderer;
    private float m_actualEnergy;

    private bool m_shieldActive = false;

    private SteamVR_Controller.Device m_device = null;
    private PickupSystem m_pickupSystem;

    // Use this for initialization
    void Start ()
    {
        if (startWithFullEnergy)
            m_actualEnergy = maxEnergy;

        if(shieldObject != null)
        {
            m_shieldCollider = shieldObject.GetComponent<Collider>();
            m_shieldRenderer = shieldObject.GetComponent<MeshRenderer>();
        }

        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
        m_pickupSystem = GetComponent<PickupSystem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_pickupSystem.m_isHandBusy)
            m_shieldActive = (m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x > 0.1f) ? true : false;

        if(m_shieldActive)
        {
            if (looseEnergyOverTime > 0.0f)
                m_actualEnergy -= Time.deltaTime * looseEnergyOverTime;

            m_shieldCollider.enabled = true;
            m_shieldRenderer.material = shieldActiveMaterial;
        }
        else
        {
            if (energyRestoration > 0.0f)
                m_actualEnergy += Time.deltaTime * energyRestoration;

            m_shieldCollider.enabled = false;
            m_shieldRenderer.material = shieldDeactivatedMaterial;
        }
	}

    public void GetHit(float damage)
    {
        Debug.Log("inflicting damage to shield");
        m_actualEnergy -= damage;
    }
}
