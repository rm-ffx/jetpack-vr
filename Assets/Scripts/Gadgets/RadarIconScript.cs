using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarIconScript : MonoBehaviour
{
    public GameObject radarPrefab;
    public Texture iconTexture;

    private static RadarProperties m_radarProp;
    private static bool _isInitialized = false;

    void Start()
    {
        if (radarPrefab != null && iconTexture != null)
        {
            GameObject obj = Instantiate(radarPrefab, transform.position, transform.rotation) as GameObject;

            // need to make a parent for the obj to have a reference of the original RadarObject position
            obj.transform.parent = this.transform;

            Renderer rend = obj.GetComponentInChildren<Renderer>();
            rend.material.mainTexture = iconTexture;
            if (!_isInitialized)
            {
                m_radarProp = FindObjectOfType<RadarProperties>();
                _isInitialized = true;
            }
            m_radarProp.AddTrackedObjects(obj);
            m_radarProp.AddRendererForRadarObjects(rend);
        }
    }
}
