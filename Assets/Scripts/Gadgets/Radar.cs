using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Script draws Objects within a Radar
 * Objects that are outside the radar are drawn
 * outside the radius of the radar
 **/

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Radar : MonoBehaviour
{
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

    private Material radarMaterial;
    private static bool m_isInuse;
    private bool m_selfIsInuse;
    private List<GameObject> m_radarObjects = new List<GameObject>();
    private List<Renderer> m_radarRenderers = new List<Renderer>();

    // Use this for initialization
    void Start()
    {
        radarMaterial = radarPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
        radarMaterial.mainTextureScale = new Vector2(0.7f, 0.7f);  // for scaling the icon on the GameObject -> 0.7 best value for this icon texture 
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (trackedObjects.Length > 0)
            createRadarObjects();
        m_isInuse = false;
        m_selfIsInuse = false;
    }

    // Update is called once per frame
    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip) && (!m_isInuse || m_selfIsInuse))
        {
            m_isInuse = true;
            m_selfIsInuse = true;
            radarCameraPlane.SetActive(true);
            float z = transform.rotation.eulerAngles.z;
            float y = transform.rotation.eulerAngles.y;
            Vector3 eulerRotation = radarCamera.rotation.eulerAngles;

            // rotate radarcamera by 180° - controllerRotation.z + HMDRotation.y
            eulerRotation.y = 180 - z + y;
            radarCamera.rotation = Quaternion.Euler(eulerRotation);

            for (int i = 0; i < m_radarObjects.Count; i++)
            {
                if (m_radarObjects[i] == null)
                {
                    m_radarObjects.RemoveAt(i);
                    m_radarRenderers.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (Vector3.Distance(m_radarObjects[i].transform.parent.position, playerPos.transform.position) > switchDistance)
                {
                    // place objects on the border
                    Vector3 a = m_radarObjects[i].transform.parent.position;
                    Vector3 b = playerPos.transform.position;
                    a.y = 0;
                    b.y = 0;

                    float dist = Vector3.Distance(a, b);
                    float alphaValue = switchDistance / dist;

                    Vector3 direction = m_radarObjects[i].transform.parent.position - playerPos.transform.position;
                    m_radarObjects[i].transform.position = playerPos.transform.position + direction.normalized * switchDistance;
                    m_radarRenderers[i].material.color = new Color(1, 1, 1, Mathf.Clamp(alphaValue, 0.2f, 1.0f));
                }
                else
                {
                    // place objects on the actual radar position
                    m_radarObjects[i].transform.position = m_radarObjects[i].transform.parent.position;
                    m_radarRenderers[i].material.color = new Color(1, 1, 1, 1.0f);
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
            Renderer rend = obj.GetComponentInChildren<Renderer>();
            rend.material.color = new Color(0, 0, 0, 0.0f);
            m_radarRenderers.Add(rend);
            m_radarObjects.Add(obj);
        }
    }
}
