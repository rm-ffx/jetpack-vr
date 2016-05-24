using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles the raycast gun gadget
/// </summary>
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class RaycastGun : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;

    [Tooltip("How many seconds the gun needs to be ready again.")]
    public float cooldown;
    [Tooltip("The damage this weapon deals.")]
    public float damage;
    [Tooltip("The impact this weapon has on Physics.")]
    public float impactForce;
    [Tooltip("The range of the gun")]
    public float range = 100.0f;
    [Tooltip("The prefab holding the impact particle system")]
    public GameObject impactParticleSystem;
    [Tooltip("The model that will be used for the gadget selector.")]
    public GameObject gadgetPreviewPrefab;
    [Tooltip("Higher multiplier results in the gun shooting farther down.")]
    public float angleMultiplier = 1.0f;
    [Tooltip("The model that will be used as pointer. Note that this is only used for visual feedback.")]
    public GameObject pointerModel;

    private SteamVR_Controller.Device m_device = null;
    private PickupSystem m_pickupSystem;
    private float m_triggerX;
    private float m_remainingCooldown;
    private Vector3 m_pointerModelLocalScale;

    // Use this for initialization
    void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        m_device = SteamVR_Controller.Input((int)trackedObj.index);
        m_pickupSystem = GetComponent<PickupSystem>();
        m_triggerX = 0.0f;
        pointerModel.SetActive(false);
        m_pointerModelLocalScale = pointerModel.transform.localScale;
    }

    void OnEnable()
    {
        pointerModel.SetActive(true);
    }

    void OnDisable()
    {
        pointerModel.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        m_device = SteamVR_Controller.Input((int)trackedObj.index);
        if (!m_pickupSystem.m_isHandBusy)
        {
            m_triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;
            if (!pointerModel.activeInHierarchy)
                pointerModel.SetActive(true);
        }
        else
            pointerModel.SetActive(false);

        if (m_triggerX >= 0.1f && m_remainingCooldown <= 0.0f)
        {
            Shoot();
            pointerModel.transform.localScale = new Vector3(m_pointerModelLocalScale.x * 10.0f, m_pointerModelLocalScale.y, m_pointerModelLocalScale.z * 10.0f);
        }
        else
        {
            m_remainingCooldown -= Time.deltaTime;
            if (m_remainingCooldown < cooldown / 2f)
                pointerModel.transform.localScale = m_pointerModelLocalScale;
        }
    }

    private void Shoot()
    {
        m_device.TriggerHapticPulse(1500);
        m_remainingCooldown = cooldown;
        RaycastHit hit;

        Vector3 newForward = (transform.forward + transform.up * -1 * angleMultiplier) / 2;
        if (Physics.Raycast(transform.position, newForward, out hit, maxDistance: range))
        {
            Instantiate(impactParticleSystem, hit.point, Quaternion.identity);
            if(hit.transform.gameObject.layer == 9)
            {
                NPCInfo npcInfo = hit.transform.root.gameObject.GetComponent<NPCInfo>();
                npcInfo.health -= damage;
                if (npcInfo.health <= 0.0f)
                {
                    if (npcInfo.puppetMaster) npcInfo.TriggerPuppetMaster(hit.collider, impactForce, hit.point, 0);
                    else
                        Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
