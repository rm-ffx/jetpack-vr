using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    public uint inventoryMaxSize;
    public float radius = 0.5f;
    public float totalOffsetInDegrees = 90.0f;
    public float itemOffsetInDegrees = 30.0f;

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
        gameObject.SetActive(false);
	}

    public void StoreItem(GameObject item)
    {
        m_storedObjects.Add(item);

        item.GetComponent<Collider>().enabled = false;
        item.GetComponent<Rigidbody>().isKinematic = true;

        if (m_storedObjects.Count >= inventoryMaxSize)
            isFull = true;

        isEmpty = false;

        OrderObjects();
    }

    public GameObject WithdrawItem(Vector3 controllerPosition)
    {
        if (m_storedObjects.Count == 0)
            return new GameObject();

        GameObject closestStoredObject = ClosestItemToPosition(controllerPosition);
        
        closestStoredObject.transform.parent = null;
        m_storedObjects.Remove(closestStoredObject);

        if (m_storedObjects.Count < inventoryMaxSize)
            isFull = false;
        if (m_storedObjects.Count == 0)
            isEmpty = true;

        OrderObjects();

        closestStoredObject.GetComponent<Collider>().enabled = true;
        closestStoredObject.GetComponent<Rigidbody>().isKinematic = false;
        return closestStoredObject;
    }

    private void OrderObjects()
    {
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Resize & rearrange inventory
        if(m_inventorySlotPositions.Count != m_storedObjects.Count)
        {
            m_inventorySlotPositions.Clear();
            Vector3 position;
            Vector3 firstPosition = transform.position + transform.forward * radius;

            float offset = totalOffsetInDegrees - itemOffsetInDegrees * 0.5f * (m_storedObjects.Count - 1);
            for(int i = 0; i < m_storedObjects.Count; i++)
            {
                position = transform.position;

                Vector3 direction = firstPosition - transform.position;
                direction = Quaternion.Euler(transform.up * offset + transform.up * (itemOffsetInDegrees * i)) * direction;
                position += direction;

                m_inventorySlotPositions.Add(position);
            }            
        }

        // Assign each Object in the inventory to a different slot and set parent
        for (int i = 0; i < m_storedObjects.Count; i++)
        {
            m_storedObjects[i].transform.position = m_inventorySlotPositions[i];
            m_storedObjects[i].transform.rotation = transform.rotation;
            m_storedObjects[i].transform.parent = transform;
        }

        transform.position = oldPosition;
        transform.rotation = oldRotation;
    }

    public void OpenInventory()
    {
        foreach (GameObject go in m_storedObjects)
        {
            go.GetComponent<ItemProperties>().Highlight(false);
            go.SetActive(true);
        }
    }

    public void CloseInventory()
    {
        foreach (GameObject go in m_storedObjects)
        {
            go.GetComponent<ItemProperties>().Highlight(false);
            go.SetActive(false);
        }
    }

    public void Highlight(Vector3 controllerPosition)
    {
        if (isEmpty)
            return;

        foreach (GameObject go in m_storedObjects)
            go.GetComponent<ItemProperties>().Highlight(false);

        ClosestItemToPosition(controllerPosition).GetComponent<ItemProperties>().Highlight(true);
    }

    private GameObject ClosestItemToPosition(Vector3 controllerPosition)
    {
        float minMagnitude = float.MaxValue;
        GameObject closestStoredObject = null;
        foreach (GameObject storedObject in m_storedObjects)
        {
            float dist = (storedObject.transform.position - controllerPosition).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                closestStoredObject = storedObject;
            }
        }
        return closestStoredObject;
    }
}
