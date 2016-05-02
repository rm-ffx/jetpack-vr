using UnityEngine;
using System.Collections;
using Valve.VR;

// Handling Jetpack Movement for VR controller

// Attach to Controller
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class JetpackMovement : MonoBehaviour {
    public float upwardMultiplier = 1.0f;
    public float downwardMultiplier = 1.0f;

    private Rigidbody m_rigidBody;

    private SteamVR_Controller.Device m_device = null; 

    private GameObject m_otherDeviceGameObject;
    private SteamVR_TrackedObject m_otherDeviceTrackedObject = null;
    private JetpackMovement m_otherDeviceJetpackMovement = null;
    //private SteamVR_Controller.Device m_otherDevice = null;

    public float triggerX { get; private set; }
    private float otherTriggerX = 0.0f;

    private bool m_isInitialized = false;

    void Start()
    {
        if ((int)GetComponent<SteamVR_TrackedObject>().index < 0)
            return;

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if(m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        // Cache Variables
        m_rigidBody = transform.parent.GetComponent<Rigidbody>();

        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);

        m_otherDeviceTrackedObject = m_otherDeviceGameObject.GetComponent<SteamVR_TrackedObject>();
        m_otherDeviceJetpackMovement = m_otherDeviceGameObject.GetComponent<JetpackMovement>();

        //if (m_otherDeviceTrackedObject.isValid)
        //    m_otherDevice = SteamVR_Controller.Input((int)m_otherDeviceTrackedObject.index);

        triggerX = 0.0f;
        otherTriggerX = 0.0f;

        m_otherDeviceGameObject.GetComponent<JetpackMovement>().ReInitialize();
        m_isInitialized = true;
    }

    public void ReInitialize()
    {
        if ((int)GetComponent<SteamVR_TrackedObject>().index < 0)
            return;

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if (m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        m_rigidBody = transform.parent.GetComponent<Rigidbody>();

        m_device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);

        m_otherDeviceTrackedObject = m_otherDeviceGameObject.GetComponent<SteamVR_TrackedObject>();
        m_otherDeviceJetpackMovement = m_otherDeviceGameObject.GetComponent<JetpackMovement>();

        //if (m_otherDeviceTrackedObject.isValid)
        //    m_otherDevice = SteamVR_Controller.Input((int)m_otherDeviceTrackedObject.index);

        triggerX = 0.0f;
        otherTriggerX = 0.0f;

        m_isInitialized = true;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!m_isInitialized)
            ReInitialize();

        // Vive controls
        triggerX = m_device.GetAxis(EVRButtonId.k_EButton_Axis1).x;

        if (m_otherDeviceTrackedObject.isValid)
            otherTriggerX = m_otherDeviceJetpackMovement.triggerX;
        else
            otherTriggerX = 0.0f;

        if (triggerX >= 0.02f)
            m_rigidBody.AddForce(Vector3.Normalize(m_device.transform.rot * Vector3.forward) * triggerX * upwardMultiplier, ForceMode.Impulse);
        else
            m_rigidBody.AddForce(Vector3.down * 0.5f * downwardMultiplier, ForceMode.Acceleration);

        if (triggerX + otherTriggerX <= 0.1f)
            m_rigidBody.AddForce(Vector3.down * 0.5f * downwardMultiplier, ForceMode.VelocityChange);
    }
}
