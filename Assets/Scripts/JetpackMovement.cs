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
    [Tooltip("Multiplier to control upward speed")]
    public float UpwardMultiplier = 1.0f;
    [Tooltip("Multiplier to control downward speed in order to make falling down more realistic")]
    public float DownwardMultiplier = 1.0f;
    [Tooltip("The model that will be used for the gadget selector")]
    public GameObject GadgetPreviewPrefab;

    private Rigidbody m_rigidBody;
    private PickupSystem m_pickupSystem;

    private SteamVR_Controller.Device m_device = null; 

    private GameObject m_otherDeviceGameObject;
    private SteamVR_TrackedObject m_otherDeviceTrackedObject = null;
    private JetpackMovement m_otherDeviceJetpackMovement = null;
    //private SteamVR_Controller.Device m_otherDevice = null;

    public float triggerX { get; private set; }
    private float otherTriggerX = 0.0f;

    // In case controlls do not work, enable this stuff again
    //private bool m_isInitialized = false;

    void Start()
    {
        //if ((int)GetComponent<SteamVR_TrackedObject>().index < 0)
        //    return;

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if(m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        // Cache Variables
        m_rigidBody = transform.parent.GetComponent<Rigidbody>();
        m_pickupSystem = GetComponent<PickupSystem>();

        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);

        m_otherDeviceTrackedObject = m_otherDeviceGameObject.GetComponent<SteamVR_TrackedObject>();
        m_otherDeviceJetpackMovement = m_otherDeviceGameObject.GetComponent<JetpackMovement>();

        triggerX = 0.0f;
        otherTriggerX = 0.0f;

        //m_otherDeviceGameObject.GetComponent<JetpackMovement>().ReInitialize();
        //m_isInitialized = true;
    }

    //public void ReInitialize()
    //{
    //    if ((int)GetComponent<SteamVR_TrackedObject>().index < 0)
    //        return;

    //    m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
    //    if (m_otherDeviceGameObject == gameObject)
    //        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

    //    m_rigidBody = transform.parent.GetComponent<Rigidbody>();

    //    m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);

    //    m_otherDeviceTrackedObject = m_otherDeviceGameObject.GetComponent<SteamVR_TrackedObject>();
    //    m_otherDeviceJetpackMovement = m_otherDeviceGameObject.GetComponent<JetpackMovement>();

    //    triggerX = 0.0f;
    //    otherTriggerX = 0.0f;

    //    m_isInitialized = true;
    //}

    // Update is called once per frame
    void FixedUpdate ()
    {
        //if (!m_isInitialized)
        //    ReInitialize();

        // Vive controls
        if (!m_pickupSystem.m_isHandBusy)
            triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;

        if (m_otherDeviceTrackedObject.isValid)
            otherTriggerX = m_otherDeviceJetpackMovement.triggerX;
        else
            otherTriggerX = 0.0f;

        if (triggerX >= 0.02f)
            m_rigidBody.AddForce(Vector3.Normalize(m_device.transform.rot * Vector3.forward) * triggerX * UpwardMultiplier, ForceMode.Impulse);
        else
            m_rigidBody.AddForce(Vector3.down * 0.5f * DownwardMultiplier, ForceMode.Acceleration);

        if (triggerX + otherTriggerX <= 0.1f)
            m_rigidBody.AddForce(Vector3.down * 0.5f * DownwardMultiplier, ForceMode.VelocityChange);
    }
}
