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

    public GameObject[] trackedObjects;
    public GameObject radarPrefab;
    public Transform radarCamera;
    public GameObject radarCameraPlane;
    public Transform playerPos;
    public float switchDistance;

    private static bool m_isInuse;
    private bool m_selfIsInuse;
    private List<GameObject> m_radarObjects;

	// Use this for initialization
	void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
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
                if (Vector3.Distance(m_radarObjects[i].transform.parent.position, playerPos.transform.position) > switchDistance)
                {
                    // switch to the border Objects
                    Vector3 direction = m_radarObjects[i].transform.parent.position - playerPos.transform.position;
                
                    m_radarObjects[i].transform.position = playerPos.transform.position + direction.normalized * switchDistance;
                    //Debug.Log("greater than switchDistance");
                }
                else
                {
                    // switch to the radar Objects
                    m_radarObjects[i].transform.position = m_radarObjects[i].transform.parent.position;
                    //Debug.Log("smaller than switchDistance");
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
            GameObject obj = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            obj.transform.parent = o.transform;
            m_radarObjects.Add(obj);
        }
        Debug.Log("Radarobjects created");
    }
}
