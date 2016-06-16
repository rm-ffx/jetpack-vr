using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles the projectile gun gadget
/// </summary>
public class ProjectileGun : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;

    [Tooltip("How many seconds the gun needs to be ready again.")]
    public float cooldown;
    [Tooltip("The model that will be used as projectile.")]
    public GameObject projectilePrefab;
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

    void OnDisable()
    {
        pointerModel.SetActive(false);
    }

    void OnEnable()
    {
        pointerModel.SetActive(true);
    }

    // Update is called once per frame
    void Update ()
    {
        m_device = SteamVR_Controller.Input((int)trackedObj.index);

        if (!m_pickupSystem.m_isHandBusy)
        { 
            if (!pointerModel.activeInHierarchy)
                pointerModel.SetActive(true);

            m_triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;

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
        else
            pointerModel.SetActive(false);
    }

    private void Shoot()
    {
        m_device.TriggerHapticPulse(1500);
        m_remainingCooldown = cooldown;
        
        Quaternion rot = Quaternion.LookRotation((transform.forward + transform.up * -1 * angleMultiplier) / 2, (transform.forward * angleMultiplier + transform.up) / 2);
        GameObject newProjectile = (GameObject)Instantiate(projectilePrefab, transform.position, rot);
        newProjectile.layer = 11;
    }
}
