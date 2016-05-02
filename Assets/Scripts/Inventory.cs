using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    public uint inventoryMaxSize;

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis sortAroundAxis;
    public float radius = 0.5f;
    public float offsetInDegrees = 30.0f;

    public bool isFull { get; private set; }
    public bool isEmpty { get; private set; }
    public List<GameObject> m_storedObjects { get; private set; }

    private List<Vector3> m_inventorySlotPositions;
    
	void Start ()
    {
        if (inventoryMaxSize > 0)
            isFull = false;
        else
            isFull = true;

        isEmpty = true;

        m_storedObjects = new List<GameObject>();
        m_inventorySlotPositions = new List<Vector3>();
	}
    
    public void StoreItem(GameObject item)
    {
        Debug.Log("STORE");
        m_storedObjects.Add(item);
        Debug.Log(item.transform.parent.gameObject.name);
        item.transform.parent = transform;
        Debug.Log(item.transform.parent.gameObject.name);

        item.GetComponent<Collider>().enabled = false;

        if (m_storedObjects.Count >= inventoryMaxSize)
            isFull = true;

        isEmpty = false;

        //Debug.Log("Stored Item, now holding " + m_storedObjects.Count);
        OrderObjects();
    }

    public GameObject WithdrawItem(Vector3 controllerPosition)
    {
        Debug.Log("WITHDRAW");
        if (m_storedObjects.Count == 0)
            return new GameObject();
        
        float minMagnitude = float.MaxValue;
        GameObject closestStoredObject = null;
        foreach(GameObject storedObject in m_storedObjects)
        {
            float dist = (storedObject.transform.position - controllerPosition).magnitude;
            if(dist < minMagnitude)
            {
                minMagnitude = dist;
                closestStoredObject = storedObject;
            }
        }
        
        closestStoredObject.transform.parent = null;
        m_storedObjects.Remove(closestStoredObject);

        if (m_storedObjects.Count < inventoryMaxSize)
            isFull = false;
        if (m_storedObjects.Count == 0)
            isEmpty = true;

        //Debug.Log("Withdrew Item, now holding " + m_storedObjects.Count);
        OrderObjects();

        closestStoredObject.GetComponent<Collider>().enabled = true;
        return closestStoredObject;
    }

    private void OrderObjects()
    {
        // Resize (if needed) & rearrange inventory
        if(m_inventorySlotPositions.Count < m_storedObjects.Count)
        {
            m_inventorySlotPositions.Clear();
            Vector3 position;

            float offset = 90.0f - offsetInDegrees * 0.5f * (m_storedObjects.Count - 1);

            for(int i = 0; i < m_storedObjects.Count; i++)
            {
                position = transform.position;
                switch (sortAroundAxis)
                {
                    case Axis.X:
                        position.y += radius * Mathf.Sin(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        position.z -= radius * Mathf.Cos(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        break;
                    case Axis.Y:
                        position.x += radius * Mathf.Sin(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        position.z += radius * Mathf.Cos(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        break;
                    case Axis.Z:
                        position.y += radius * Mathf.Sin(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        position.x += radius * Mathf.Cos(offset * Mathf.Deg2Rad + i * Mathf.Deg2Rad * offsetInDegrees);
                        break;
                }

                m_inventorySlotPositions.Add(position);
            }
        }

        // Assign each Object in the inventory to a different slot
        for(int i = 0; i < m_storedObjects.Count; i++)
        {
            m_storedObjects[i].transform.position = m_inventorySlotPositions[i];
        }
    }

    public void OpenInventory()
    {
        foreach (GameObject go in m_storedObjects)
            go.SetActive(true);
    }

    public void CloseInventory()
    {
        foreach (GameObject go in m_storedObjects)
            go.SetActive(false);
    }
}
