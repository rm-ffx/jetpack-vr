using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the gadget selection
/// </summary>
public class GadgetSelector : MonoBehaviour
{
    [Tooltip("Connect all gadget scripts to this list. Gadgets should have a display model connected")]
    public List<MonoBehaviour> Gadgets;
    [Tooltip("The maximum ammount of gadgets")]
    public int MaxGadgets = 8;
    [Tooltip("How far the gadgets will be from the activation point")]
    public float Distance = 0.5f;

    private Vector3[] m_calculatedPositions;
    private GameObject[] m_gadgetObjects;

	// Use this for initialization
	void Start ()
    {
        m_calculatedPositions = new Vector3[MaxGadgets];
        m_gadgetObjects = new GameObject[MaxGadgets];

        SetPositions();

        Vector3 position;
        for(int i = 0; i < MaxGadgets; i++)
        {
            position = m_calculatedPositions[i];

            if(i < Gadgets.Count)
                switch (Gadgets[i].GetType().ToString())
                {
                    case "JetpackMovement":
                        m_gadgetObjects[i] = Instantiate(Gadgets[i].GetComponent<JetpackMovement>().GadgetPreviewPrefab);
                        break;
                    case "Gun":
                        m_gadgetObjects[i] = Instantiate(Gadgets[i].GetComponent<Gun>().GadgetPreviewPrefab);
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

        foreach (MonoBehaviour gadget in Gadgets)
            gadget.enabled = false;
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

        Vector3 firstPosition = transform.position + transform.up * Distance;
        Vector3 position;
        Vector3 direction;
        float offsetInDegrees = (float)360 / MaxGadgets;

        for (int i = 0; i < MaxGadgets; i++)
        {
            position = transform.position;
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
        for (int i = 0; i < MaxGadgets; i++)
        {
            if(m_gadgetObjects[i] != null)
            {
                m_gadgetObjects[i].transform.position = transform.position + m_calculatedPositions[i];
                m_gadgetObjects[i].transform.parent = transform;
            }
        }

        // Apply Y rotation
        transform.rotation = Quaternion.Euler(oldRotationEuler);
    }

    public void OpenGadgetSelector()
    {
        SetPositions();

        for (int i = 0; i < MaxGadgets; i++)
        {
            m_gadgetObjects[i].transform.parent = transform.parent;
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

    public void CloseGadgetSelector(Vector3 controllerPosition)
    {
        int closestIndex = ClosestIndexToPosition(controllerPosition);

        for (int i = 0; i < MaxGadgets; i++)
        {
            m_gadgetObjects[i].GetComponent<MeshRenderer>().enabled = false;
            //m_gadgetObjects[i].GetComponent<Collider>().enabled = false;

            if(i < Gadgets.Count)
            {
                if (i == closestIndex)
                    Gadgets[i].enabled = true;
                else
                    Gadgets[i].enabled = false;
            }
        }
    }
    
	private GameObject ClosestObjectToPosition(Vector3 controllerPosition)
    {
        float minMagnitude = float.MaxValue;
        GameObject closestObject = null;
        foreach (GameObject gadgetObject in m_gadgetObjects)
        {
            float dist = (gadgetObject.transform.position - controllerPosition).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                closestObject = gadgetObject;
            }
        }
        return closestObject;
    }

    private int ClosestIndexToPosition(Vector3 controllerPosition)
    {
        float minMagnitude = float.MaxValue;
        int closestIndex = -1;
        for(int i = 0; i < MaxGadgets; i++)
        {
            float dist = (m_gadgetObjects[i].transform.position - controllerPosition).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public void Highlight(Vector3 controllerPosition)
    {
        foreach(GameObject go in m_gadgetObjects)
            go.GetComponent<MeshRenderer>().material.color = Color.cyan;
        ClosestObjectToPosition(controllerPosition).GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
