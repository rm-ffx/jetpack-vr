using UnityEngine;
using System.Collections;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Shield : MonoBehaviour
{
    [Tooltip("The maximum ammount of energy the shield can have")]
    public float maxEnergy = 100.0f;
    [Tooltip("Wheter or not the shield starts off with full energy")]
    public bool startWithFullEnergy = true;
    [Tooltip("Wheter or not the shield looses energy when hit by a projectile")]
    public bool looseEnergyOnHit = false;
    [Tooltip("How fast energy is consumed while the shield is active. Setting this value to 0 results in the shield not losing energy over time")]
    public float looseEnergyOverTime = 0.0f;
    [Tooltip("How fast energy regenerates while the shield is deactivated. Setting this value to 0 results in the shield not regenerating energy over time")]
    public float energyRegeneration = 0.0f;

    [Tooltip("The GameObject holding the shield's model and collider")]
    public GameObject shieldObject;
    [Tooltip("The material that will be used when the shield is active")]
    public Material shieldActiveMaterial;
    [Tooltip("The material that will be used when the shield is deactivated")]
    public Material shieldDeactivatedMaterial;
    [Tooltip("The model that will be used for the gadget selector")]
    public GameObject gadgetPreviewPrefab;

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

        shieldObject.SetActive(false);

        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
        m_pickupSystem = GetComponent<PickupSystem>();
    }
	
    void OnEnable()
    {
        shieldObject.SetActive(true);
    }

    void OnDisable()
    {
        shieldObject.SetActive(false);
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
            if (energyRegeneration > 0.0f)
                m_actualEnergy += Time.deltaTime * energyRegeneration;

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
