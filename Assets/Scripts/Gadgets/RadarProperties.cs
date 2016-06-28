using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarProperties : MonoBehaviour
{
    [Tooltip("List of the objects tracked on the radar.")]
    public GameObject[] trackedObjects;
    //[Tooltip("The list of prefabs that will be used to display on the radar. Use the same order as in Tracked Objects list.")]
    //public GameObject[] radarPrefabs;
    //[Tooltip("The prefab that will be used to display on the radar if RadarPrefabs list is not filled properly.")]
    //public GameObject defaultRadarPrefab;
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
            createRadarObjects();

        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.Initialize();
    }

    public void AddTrackedObjects(GameObject obj)
    {
        radarObjects.Add(obj);
        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.RefreshRadarObjects();
        radarCamera.depthTextureMode = DepthTextureMode.None;
    }

    public void AddRendererForRadarObjects(Renderer rend)
    {
        radarRenderers.Add(rend);
    }

    void createRadarObjects()
    {
        radarObjects = new List<GameObject>();
        radarRenderers = new List<Renderer>();

        GameObject go;

        for(int i = 0; i < trackedObjects.Length; i++)
        {
            go = trackedObjects[i];
            if (go == null)
                continue;

            //GameObject obj;
            //if (i < radarPrefabs.Length && radarPrefabs[i] != null)
            //    obj = Instantiate(radarPrefabs[i], go.transform.position, Quaternion.identity) as GameObject;
            //else
            //{
            //    obj = Instantiate(defaultRadarPrefab, go.transform.position, Quaternion.identity) as GameObject;
            //}

            //obj.transform.parent = go.transform;
            //radarObjects.Add(obj);

            //Renderer rend = go.GetComponentInChildren<Renderer>();
            //Renderer rend = GameObject.FindGameObjectWithTag("RadarIcon").GetComponent<Renderer>();
            //rend.material.color = new Color(0, 0, 0, 0.0f);
            //radarRenderers.Add(rend);
        }
    }
}
