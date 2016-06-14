using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarProperties : MonoBehaviour
{
    [Tooltip("List of the objects tracked on the radar.")]
    public GameObject[] trackedObjects;
    [Tooltip("The prefab that will be used to display on the radar.")]
    public GameObject radarPrefab;
    [Tooltip("The camera that renders the radar.")]
    public Transform radarCamera;
    [Tooltip("Reference to the player position.")]
    public Transform playerPos;
    [Tooltip("Radius of the radar.")]
    public float switchDistance;

    private Material radarMaterial;

    public List<GameObject> radarObjects { get; private set; }
    public List<Renderer> radarRenderers { get; private set; }

    // Use this for initialization
    void Start ()
    {
        if (trackedObjects.Length > 0)
            createRadarObjects();

        radarMaterial = radarPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
        radarMaterial.mainTextureScale = new Vector2(0.7f, 0.7f);  // for scaling the icon on the GameObject -> 0.7 best value for this icon texture 

        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.Initialize();
    }

    void createRadarObjects()
    {
        radarObjects = new List<GameObject>();
        radarRenderers = new List<Renderer>();

        foreach (GameObject o in trackedObjects)
        {
            if (o == null)
                continue;

            GameObject obj = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            obj.transform.parent = o.transform;
            radarObjects.Add(obj);

            Renderer rend = obj.GetComponentInChildren<Renderer>();
            rend.material.color = new Color(0, 0, 0, 0.0f);
            radarRenderers.Add(rend);
        }
    }
}
