using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the gadget selection
/// </summary>
public class GadgetSelector : MonoBehaviour
{
    [Tooltip("Connect all gadget scripts to this list. Gadgets should have a display model connected.")]
    public List<MonoBehaviour> gadgets;
    [Tooltip("The maximum ammount of gadgets.")]
    public int maxGadgets = 8;
    [Tooltip("How far the gadgets will be from the activation point.")]
    public float radialDistance = 0.5f;
    [Tooltip("How far the gadgets will be from the activation point.")]
    public float forwardDistance = 0.18f;

    private Vector3[] m_calculatedPositions;
    private GameObject[] m_gadgetObjects;

	// Use this for initialization
	void Start ()
    {
        Initialize();
    }

    void Initialize()
    {
        m_calculatedPositions = new Vector3[maxGadgets];
        m_gadgetObjects = new GameObject[maxGadgets];

        SetPositions();

        Vector3 position;
        for (int i = 0; i < maxGadgets; i++)
        {
            position = m_calculatedPositions[i];

            if (i < gadgets.Count)
                switch (gadgets[i].GetType().ToString())
                {
                    case "JetpackMovement":
                        m_gadgetObjects[i] = Instantiate(gadgets[i].GetComponent<JetpackMovement>().gadgetPreviewPrefab);
                        break;
                    case "ProjectileGun":
                        m_gadgetObjects[i] = Instantiate(gadgets[i].GetComponent<ProjectileGun>().gadgetPreviewPrefab);
                        break;
                    case "RaycastGun":
                        m_gadgetObjects[i] = Instantiate(gadgets[i].GetComponent<RaycastGun>().gadgetPreviewPrefab);
                        break;
                    case "Shield":
                        m_gadgetObjects[i] = Instantiate(gadgets[i].GetComponent<Shield>().gadgetPreviewPrefab);
                        break;
                    default:
                        m_gadgetObjects[i] = GetDefaultGadgetPreview();
                        break;
                }
            else
                m_gadgetObjects[i] = GetDefaultGadgetPreview();

            m_gadgetObjects[i].transform.position = position;
            m_gadgetObjects[i].transform.parent = transform;
            m_gadgetObjects[i].GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (MonoBehaviour gadget in gadgets)
            gadget.enabled = false;
    }

    public void AddGadget(MonoBehaviour gadget)
    {
        if(gadgets.Count < maxGadgets)
        {
            switch (gadget.GetType().ToString())
            {
                case "JetpackMovement":
                    JetpackMovement gadgetJM = gadget.GetComponent<JetpackMovement>();
                    JetpackMovement newGadgetJM = gameObject.AddComponent<JetpackMovement>();
                    newGadgetJM.upwardMultiplier = gadgetJM.upwardMultiplier;
                    newGadgetJM.gadgetPreviewPrefab = gadgetJM.gadgetPreviewPrefab;
                    gadgets.Add(newGadgetJM);
                    newGadgetJM.enabled = false;
                    break;
                case "ProjectileGun":
                    ProjectileGun gadgetPG = gadget.GetComponent<ProjectileGun>();
                    ProjectileGun newGadgetPG = gameObject.AddComponent<ProjectileGun>();
                    newGadgetPG.cooldown = gadgetPG.cooldown;
                    newGadgetPG.projectilePrefab = gadgetPG.projectilePrefab;
                    newGadgetPG.gadgetPreviewPrefab = gadgetPG.gadgetPreviewPrefab;
                    newGadgetPG.angleMultiplier = gadgetPG.angleMultiplier;
                    newGadgetPG.pointerModel = gadgetPG.pointerModel;
                    gadgets.Add(newGadgetPG);
                    newGadgetPG.enabled = false;
                    break;
                case "RaycastGun":
                    RaycastGun gadgetRG = gadget.GetComponent<RaycastGun>();
                    RaycastGun newGadgetRG = gameObject.AddComponent<RaycastGun>();
                    newGadgetRG.cooldown = gadgetRG.cooldown;
                    newGadgetRG.damage = gadgetRG.damage;
                    newGadgetRG.impactForce = gadgetRG.impactForce;
                    newGadgetRG.range = gadgetRG.range;
                    newGadgetRG.impactParticleSystem = gadgetRG.impactParticleSystem;
                    newGadgetRG.gadgetPreviewPrefab = gadgetRG.gadgetPreviewPrefab;
                    newGadgetRG.angleMultiplier = gadgetRG.angleMultiplier;
                    newGadgetRG.pointerModel = GetComponent<GadgetReferences>().raycastGunPointer;
                    gadgets.Add(newGadgetRG);
                    newGadgetRG.enabled = false;
                    break;
                case "Shield":
                    Shield gadgetSh = gadget.GetComponent<Shield>();
                    Shield newGadgetSh = gameObject.AddComponent<Shield>();
                    newGadgetSh.maxEnergy = gadgetSh.maxEnergy;
                    newGadgetSh.startWithFullEnergy = gadgetSh.startWithFullEnergy;
                    newGadgetSh.looseEnergyOnHit = gadgetSh.looseEnergyOnHit;
                    newGadgetSh.looseEnergyOverTime = gadgetSh.looseEnergyOverTime;
                    newGadgetSh.energyRegeneration = gadgetSh.energyRegeneration;
                    newGadgetSh.shieldObject = GetComponent<GadgetReferences>().shield;
                    newGadgetSh.shieldActiveMaterial = gadgetSh.shieldActiveMaterial;
                    newGadgetSh.shieldDeactivatedMaterial = gadgetSh.shieldDeactivatedMaterial;
                    newGadgetSh.gadgetPreviewPrefab = gadgetSh.gadgetPreviewPrefab;
                    break;
                default:
                    Debug.LogError("Gadget not listed, unable to add");
                    break;
            }
            Initialize();
        }
    }

    private GameObject GetDefaultGadgetPreview()
    {
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newObj.GetComponent<Collider>().isTrigger = true;
        newObj.GetComponent<Collider>().enabled = false;
        newObj.GetComponent<MeshRenderer>().material.color = Color.cyan;
        newObj.GetComponent<MeshRenderer>().enabled = false;
        newObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return newObj;
    }

    private void SetPositions()
    {
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        Vector3 position = transform.position + transform.forward * forwardDistance;
        Vector3 firstPosition = position + transform.up * radialDistance;
        Vector3 direction;
        float offsetInDegrees = (float)360 / maxGadgets;

        for (int i = 0; i < maxGadgets; i++)
        {
            position = transform.position + transform.forward * forwardDistance;
            direction = firstPosition - transform.position;
            direction = Quaternion.Euler(transform.forward * (offsetInDegrees * i)) * direction;
            position += direction;
            m_calculatedPositions[i] = position;
        }

        transform.position = oldPosition;
        // To ensure the GadgetPreviews will only be rotated around the Y axis, first rotate around XZ, then set positions & parent and rotate around Y afterwards
        Vector3 oldRotationEuler = oldRotation.eulerAngles;
        Vector3 oldRotationXZ = new Vector3(oldRotationEuler.x, 0.0f, oldRotationEuler.z);
        // Apply XZ rotation
        transform.rotation = Quaternion.Euler(oldRotationXZ);

        // Set positions & parent
        for (int i = 0; i < maxGadgets; i++)
        {
            if(m_gadgetObjects[i] != null)
            {
                m_gadgetObjects[i].transform.position = transform.position + m_calculatedPositions[i];
                m_gadgetObjects[i].transform.parent = transform;
            }
        }

        // Apply Y rotation of Camera (head) object to ensure GadgetPreview is rotated properly
        transform.rotation = Quaternion.Euler(oldRotationEuler);

        for(int i = 0; i < maxGadgets; i++)
        {
            if(m_gadgetObjects[i] != null)
            {
                oldPosition = m_gadgetObjects[i].transform.position;
                m_gadgetObjects[i].transform.position = Vector3.zero;
                
                oldRotationEuler = transform.parent.Find("Camera (head)").rotation.eulerAngles;
                m_gadgetObjects[i].transform.rotation = Quaternion.Euler(new Vector3(0.0f, oldRotationEuler.y, 0.0f));

                m_gadgetObjects[i].transform.position = oldPosition;
            }
        }
    }

    public void OpenGadgetSelector()
    {
        SetPositions();

        for (int i = 0; i < maxGadgets; i++)
        {
            //m_gadgetObjects[i].transform.parent = transform.parent;
            m_gadgetObjects[i].GetComponent<MeshRenderer>().enabled = true;
            //m_gadgetObjects[i].GetComponent<Collider>().enabled = true;
        }
    }

    //public void CancelGadgetSelector()
    //{
    //    for (int i = 0; i < MaxGadgets; i++)
    //    {
    //        m_gadgetObjects[i].GetComponent<MeshRenderer>().enabled = false;

    //        if (i < Gadgets.Count)
    //        {
    //            Gadgets[i].enabled = false;
    //        }
    //    }
    //}

    public void CloseGadgetSelector(Vector3 controllerPosition, Vector2 axisOffset)
    {
        int closestIndex = ClosestIndexToPosition(controllerPosition, axisOffset);

        for (int i = 0; i < maxGadgets; i++)
        {
            m_gadgetObjects[i].GetComponent<MeshRenderer>().enabled = false;

            if(i < gadgets.Count)
            {
                if (i == closestIndex)
                    gadgets[i].enabled = true;
                else
                    gadgets[i].enabled = false;
            }
        }
    }
    
	private GameObject ClosestObjectToPosition(Vector3 controllerPosition, Vector2 axisOffset)
    {
        Vector3 xOffset = transform.right * axisOffset.x * 5;
        Vector3 alteredControllerPosition = new Vector3(controllerPosition.x + xOffset.x, controllerPosition.y + axisOffset.y * 5, controllerPosition.z + xOffset.z);
        float minMagnitude = float.MaxValue;
        GameObject closestObject = null;
        foreach (GameObject gadgetObject in m_gadgetObjects)
        {
            float dist = (gadgetObject.transform.position - alteredControllerPosition).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                closestObject = gadgetObject;
            }
        }
        return closestObject;
    }

    private int ClosestIndexToPosition(Vector3 controllerPosition, Vector2 axisOffset)
    {
        Vector3 xOffset= transform.right * axisOffset.x * 5;
        Vector3 alteredControllerPosition = new Vector3(controllerPosition.x + xOffset.x, controllerPosition.y + axisOffset.y * 5, controllerPosition.z + xOffset.z);
        float minMagnitude = float.MaxValue;
        int closestIndex = -1;
        for(int i = 0; i < maxGadgets; i++)
        {
            float dist = (m_gadgetObjects[i].transform.position - alteredControllerPosition).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public void Highlight(Vector3 controllerPosition, Vector2 axisOffset)
    {
        foreach(GameObject go in m_gadgetObjects)
            go.GetComponent<MeshRenderer>().material.color = Color.cyan;
        ClosestObjectToPosition(controllerPosition, axisOffset).GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
