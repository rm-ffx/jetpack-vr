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
    
    [Tooltip("The plane to which will be rendered.")]
    public GameObject radarCameraPlane;

    private RadarProperties m_radarProperties;

    private Transform m_radarCamera;
    private Transform m_playerPos;
    private float m_switchDistance;

    private List<GameObject> m_radarObjects;
    private List<Renderer> m_radarRenderers;

    private static bool m_isInuse;
    private bool m_selfIsInuse;

    // Use this for initialization, not Start() to make sure RadarProperties is set up properly before copying values
    public void Initialize()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        m_radarProperties = transform.parent.GetComponent<RadarProperties>();

        m_radarCamera = m_radarProperties.radarCamera;
        m_playerPos = m_radarProperties.playerPos;
        m_switchDistance = m_radarProperties.switchDistance;

        m_radarObjects = m_radarProperties.radarObjects;
        m_radarRenderers = m_radarProperties.radarRenderers;

        m_isInuse = false;
        m_selfIsInuse = false;
    }

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
            Vector3 eulerRotation = m_radarCamera.rotation.eulerAngles;

            // rotate radarcamera by 180° - controllerRotation.z + HMDRotation.y
            eulerRotation.y = 180 - z + y;
            m_radarCamera.rotation = Quaternion.Euler(eulerRotation);

            for (int i = 0; i < m_radarObjects.Count; i++)
            {
                if (m_radarObjects[i] == null)
                {
                    // Remove null entries (e.g. destroyed objects, dead enemies)
                    m_radarObjects.RemoveAt(i);
                    m_radarRenderers.RemoveAt(i);
                    // Since an object is removed, make sure to reduce index to ensure all objects get checked.
                    i--;
                    continue;
                }
                else if (Vector3.Distance(m_radarObjects[i].transform.parent.position, m_playerPos.transform.position) > m_switchDistance)
                {
                    // place objects on the border
                    Vector3 a = m_radarObjects[i].transform.parent.position;
                    Vector3 b = m_playerPos.transform.position;
                    a.y = 0;
                    b.y = 0;

                    float dist = Vector3.Distance(a, b);
                    float alphaValue = m_switchDistance / dist;

                    Vector3 direction = m_radarObjects[i].transform.parent.position - m_playerPos.transform.position;
                    m_radarObjects[i].transform.position = m_playerPos.transform.position + direction.normalized * m_switchDistance;
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
}
