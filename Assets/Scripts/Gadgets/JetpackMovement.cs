using UnityEngine;
using System.Collections;
using Valve.VR;

/// <summary>
/// Handles Jetpack Movement for VR controller
/// </summary>
// Attach to Controller
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class JetpackMovement : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;

    [Tooltip("Multiplier to control upward speed.")]
    public float upwardMultiplier = 1.0f;
    [Tooltip("The model that will be used for the gadget selector.")]
    public GameObject gadgetPreviewPrefab;

    private Rigidbody m_rigidBody;
    private PickupSystem m_pickupSystem;

    private GameObject m_otherDeviceGameObject;

    private SteamVR_Controller.Device m_device = null; 
    private JetpackMovement m_otherDeviceJetpackMovement = null;

    public float triggerX { get; private set; }
    private float otherTriggerX = 0.0f;
    private float m_downwardMultiplier = 1.0f;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if(m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        // Cache Variables
        m_rigidBody = transform.parent.GetComponent<Rigidbody>();
        m_pickupSystem = GetComponent<PickupSystem>();

        m_device = SteamVR_Controller.Input((int)trackedObj.index);
        m_otherDeviceJetpackMovement = m_otherDeviceGameObject.GetComponent<JetpackMovement>();

        triggerX = 0.0f;
        otherTriggerX = 0.0f;
        m_downwardMultiplier = transform.parent.GetComponent<JetpackMovementDownwardMultiplier>().downwardMultiplier;
    }

    void FixedUpdate ()
    {
        m_device = SteamVR_Controller.Input((int)trackedObj.index);

        // Vive controls
        if (!m_pickupSystem.m_isHandBusy)
            triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;
        else
            triggerX = 0.0f;

        if(m_otherDeviceJetpackMovement.enabled)
            otherTriggerX = m_otherDeviceJetpackMovement.triggerX;
        else
            otherTriggerX = 0.0f;

        if (triggerX >= 0.02f)
            m_rigidBody.AddForce(Vector3.Normalize(m_device.transform.rot * Vector3.forward) * triggerX * upwardMultiplier, ForceMode.Impulse);
        else if (triggerX + otherTriggerX <= 0.1f)
            m_rigidBody.AddForce(Vector3.down * m_downwardMultiplier, ForceMode.VelocityChange);
        else
            m_rigidBody.AddForce(Vector3.down * 0.5f * m_downwardMultiplier, ForceMode.Acceleration);

    }
}
