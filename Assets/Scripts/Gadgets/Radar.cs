using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Script draws Objects within a Radar
 * Objects that are outside the radar are drawn
 * outside the radius of the radar
 **/

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Radar : MonoBehaviour {
    SteamVR_TrackedObject trackedObj;

    [Tooltip("List of the objects tracked on the radar.")]
    public GameObject[] trackedObjects;
    [Tooltip("The prefab that will be used to display on the radar.")]
    public GameObject radarPrefab;
    [Tooltip("The camera that renders the radar.")]
    public Transform radarCamera;
    [Tooltip("The plane to which will be rendered.")]
    public GameObject radarCameraPlane;
    [Tooltip("Reference to the player position.")]
    public Transform playerPos;
    [Tooltip("Radius of the radar.")]
    public float switchDistance;

    private static bool m_isInuse;
    private bool m_selfIsInuse;
    private List<GameObject> m_radarObjects;

	// Use this for initialization
	void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if(trackedObjects.Length > 0)
            createRadarObjects();
        m_isInuse = false;
        m_selfIsInuse = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if(device.GetPress(SteamVR_Controller.ButtonMask.Grip) && (!m_isInuse || m_selfIsInuse))
        {
            m_isInuse = true;
            m_selfIsInuse = true;
            radarCameraPlane.SetActive(true);
            float z = transform.rotation.eulerAngles.z;
            Vector3 eulerRotation = radarCamera.rotation.eulerAngles;

            // rotate radarcamera by 180° - controllerRotation.z + HMDRotation.y
            eulerRotation.y = 180 - z + playerPos.parent.rotation.eulerAngles.y;
            radarCamera.rotation = Quaternion.Euler(eulerRotation);
        
            for (int i = 0; i < m_radarObjects.Count; i++)
            {
                if (m_radarObjects[i] == null)
                {
                    m_radarObjects.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (Vector3.Distance(m_radarObjects[i].transform.parent.position, playerPos.transform.position) > switchDistance)
                {
                    // place objects on the border
                    Vector3 direction = m_radarObjects[i].transform.parent.position - playerPos.transform.position;
                    m_radarObjects[i].transform.position = playerPos.transform.position + direction.normalized * switchDistance;
                }
                else
                {
                    // place objects on the actual radar position
                    m_radarObjects[i].transform.position = m_radarObjects[i].transform.parent.position;
                }
            }
        }
        else
        {
            m_isInuse = false;
            m_selfIsInuse = false;
            radarCameraPlane.SetActive(false);
        }
    }

    void createRadarObjects()
    {
        m_radarObjects = new List<GameObject>();

        foreach (GameObject o in trackedObjects)
        {
            if (o == null)
                continue;

            GameObject obj = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            obj.transform.parent = o.transform;
            m_radarObjects.Add(obj);
        }
    }
}
