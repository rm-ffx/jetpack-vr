using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles the raycast gun gadget
/// </summary>
public class RaycastGun : MonoBehaviour
{
    [Tooltip("How many seconds the gun needs to be ready again")]
    public float Cooldown;
    [Tooltip("The damage this weapon deals")]
    public float Damage;
    [Tooltip("The model that will be used for the gadget selector")]
    public GameObject GadgetPreviewPrefab;

    private SteamVR_Controller.Device m_device = null;
    private PickupSystem m_pickupSystem;
    private float m_triggerX;
    private float m_remainingCooldown;

    // Use this for initialization
    void Start ()
    {
        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
        m_pickupSystem = GetComponent<PickupSystem>();
        m_triggerX = 0.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_pickupSystem.m_isHandBusy)
            m_triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;

        if (m_triggerX >= 0.1f && m_remainingCooldown <= 0.0f)
            Shoot();
        else
            m_remainingCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        m_remainingCooldown = Cooldown;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.forward.normalized, transform.forward, out hit, maxDistance: 100.0f))
        {
            if(hit.transform.gameObject.layer == 9)
            {
                NPCInfo npcInfo = hit.transform.gameObject.GetComponent<NPCInfo>();
                npcInfo.health -= Damage;
                if (npcInfo.health <= 0.0f)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
