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

        m_radarCamera = m_radarProperties.radarCamera.transform;
        m_playerPos = m_radarProperties.playerPos;
        m_switchDistance = m_radarProperties.switchDistance;

        m_radarObjects = m_radarProperties.radarObjects;
        m_radarRenderers = m_radarProperties.radarRenderers;

        m_isInuse = false;
        m_selfIsInuse = false;
    }

    public void RefreshRadarObjects()
    {
        m_radarObjects = m_radarProperties.radarObjects;
        m_radarRenderers = m_radarProperties.radarRenderers;
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
                Vector3 a = m_radarObjects[i].transform.parent.position;      
                Vector3 b = m_playerPos.transform.position;
                a.y = 0;
                b.y = 0;

                Vector3 direction = a - b;
                float dist = Vector3.Distance(a, b);

                if (m_radarObjects[i] == null)
                {
                    // Remove null entries (e.g. destroyed objects, dead enemies)
                    m_radarObjects.RemoveAt(i);
                    m_radarRenderers.RemoveAt(i);
                    // Since an object is removed, make sure to reduce index to ensure all objects get checked.
                    i--;
                    continue;
                }
                else if (dist > m_switchDistance)
                {
                    // place objects on the border
                    float alphaValue = m_switchDistance / dist;

                    m_radarObjects[i].transform.position = b + direction.normalized * m_switchDistance;
                    m_radarRenderers[i].material.color = new Color(1, 1, 1, Mathf.Clamp(alphaValue, 0.2f, 0.5f));
                }
                else
                {
                    // place objects on the actual radar position
                    m_radarObjects[i].transform.position = b + direction.normalized * dist;

                    Debug.Log("actual radar position");
                    m_radarRenderers[i].material.color = new Color(1, 1, 1, 1.0f);
                }

                // setting a fixed y-position of all radar objects to improve performance
                Vector3 fixedHeightPos = new Vector3(m_radarObjects[i].transform.position.x, m_radarProperties.radarObjectToCameraDistance + m_playerPos.transform.position.y, m_radarObjects[i].transform.position.z);
                m_radarObjects[i].transform.position = fixedHeightPos;
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
