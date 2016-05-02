using UnityEngine;
using System.Collections;
using Valve.VR;

// Identify the connected HMD

public class VRInputManager : MonoBehaviour {

    public enum VRDetectedHardware
    {
        None = 0,
        HTCVive = 1,
        Oculus = 2,
    }

    public static VRDetectedHardware DetectedHardware = 0;

    private bool m_isInitialized = false;

    public SteamVR_TrackedObject leftDevice, rightDevice;

    public static SteamVR_Controller.Device m_leftDevice { get; private set; }
    public static SteamVR_Controller.Device m_rightDevice { get; private set; }

    // Use this for initialization
    void Start ()
    {
	}
	
    public void ReInitialize()
    {
        Debug.Log("New controller detected, re-initializing controller-mapping");
        m_leftDevice = null;
        m_rightDevice = null;
        Initialize();
    }

    private void Initialize()
    {
        //if (OpenVR.IsHmdPresent())
        //{
        //    string modelNumber = SteamVR.instance.hmd_ModelNumber;
        //    if (modelNumber == "Vive DVT")
        //        DetectedHardware = VRDetectedHardware.HTCVive;
        //    else if (modelNumber == "Oculus")
        //        DetectedHardware = VRDetectedHardware.Oculus;
        //    else
        //    { 
        //        Debug.LogError("Unknown HMD Detected");
        //        DetectedHardware = VRDetectedHardware.None;
        //    }
        //}

        // Save controller connection
        if (leftDevice.isValid && rightDevice.isValid)
        {
            m_leftDevice = SteamVR_Controller.Input((int)leftDevice.index);
            m_rightDevice = SteamVR_Controller.Input((int)rightDevice.index);
        }
        else if (leftDevice.isValid || rightDevice.isValid)
        {
            m_leftDevice = SteamVR_Controller.Input((int)leftDevice.index);
        }
        else
            return;

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_isInitialized)
            Initialize();

        // Check if a Re-Initialize is required
        if (m_leftDevice == m_rightDevice) m_isInitialized = false;
    }
}
