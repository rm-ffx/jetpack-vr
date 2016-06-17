using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarProperties : MonoBehaviour
{
    [Tooltip("List of the objects tracked on the radar.")]
    public GameObject[] trackedObjects;
    [Tooltip("The list of prefabs that will be used to display on the radar. Use the same order as in Tracked Objects list.")]
    public GameObject[] radarPrefabs;
    [Tooltip("The prefab that will be used to display on the radar if RadarPrefabs list is not filled properly.")]
    public GameObject defaultRadarPrefab;
    [Tooltip("The camera that renders the radar.")]
    public Transform radarCamera;
    [Tooltip("Reference to the player position.")]
    public Transform playerPos;
    [Tooltip("Radius of the radar.")]
    public float switchDistance;

    public List<GameObject> radarObjects { get; private set; }
    public List<Renderer> radarRenderers { get; private set; }

    // Use this for initialization
    void Start ()
    {
        if (trackedObjects.Length > 0)
            createRadarObjects();

        // Not quite sure how to rework this to work on multiple radarobjects / multiple materials or why it's even needed.
        // Might be best to scale the texture manually and not in the code, since it would be ugly to hardcode all material scalings and I'm not sure if there's a generic solution (at least no performant one)
        //Material radarMaterial = defaultRadarPrefab.GetComponentInChildren<Renderer>().sharedMaterial;
        //radarMaterial.mainTextureScale = new Vector2(0.7f, 0.7f);  // for scaling the icon on the GameObject -> 0.7 best value for this icon texture 

        foreach (Radar radar in GetComponentsInChildren<Radar>())
            radar.Initialize();
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

            GameObject obj;
            if(i < radarPrefabs.Length && radarPrefabs[i] != null)
                obj = Instantiate(radarPrefabs[i], go.transform.position, Quaternion.identity) as GameObject;  
            else
            {
                obj = Instantiate(defaultRadarPrefab, go.transform.position, Quaternion.identity) as GameObject;
            }

            obj.transform.parent = go.transform;
            radarObjects.Add(obj);

            Renderer rend = obj.GetComponentInChildren<Renderer>();
            rend.material.color = new Color(0, 0, 0, 0.0f);
            radarRenderers.Add(rend);
        }
    }
}
