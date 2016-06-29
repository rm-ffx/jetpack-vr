using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarProperties : MonoBehaviour
{
    [Tooltip("List of the objects tracked on the radar.")]
    public GameObject[] trackedObjects;
    [Tooltip("The camera that renders the radar.")]
    public Camera radarCamera;
    [Tooltip("Reference to the player position.")]
    public Transform playerPos;
    [Tooltip("Radius of the radar.")]
    public float switchDistance;
    [Tooltip("Y - distance of the camera objects")]
    public float radarObjectToCameraDistance;

    public List<GameObject> radarObjects { get; private set; }
    public List<Renderer> radarRenderers { get; private set; }

    // Use this for initialization
    public void Start ()
    {
        if (trackedObjects.Length > 0)
        {
            radarObjects = new List<GameObject>();
            radarRenderers = new List<Renderer>();
        }
            

        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.Initialize();
        radarCamera.depthTextureMode = DepthTextureMode.None;
    }

    public void AddTrackedObjects(GameObject obj)
    {
        radarObjects.Add(obj);
        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.RefreshRadarObjects();
    }

    public void AddRendererForRadarObjects(Renderer rend)
    {
        radarRenderers.Add(rend);
    }
}
