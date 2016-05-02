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
        m_storedObjects.Add(item);
        if (m_storedObjects.Count >= inventoryMaxSize)
            isFull = true;

        isEmpty = false;

        item.transform.parent = transform;
        //Debug.Log("Stored Item, now holding " + m_storedObjects.Count);
        OrderObjects();
    }

    public GameObject WithdrawItem(int index = 0)
    {
        GameObject go = m_storedObjects[index];
        m_storedObjects.Remove(go);

        if (m_storedObjects.Count < inventoryMaxSize)
            isFull = false;
        if (m_storedObjects.Count == 0)
            isEmpty = true;

        //Debug.Log("Withdrew Item, now holding " + m_storedObjects.Count);
        OrderObjects();
        return go;        
    }

    public void WithdrawItem(GameObject item)
    {
        m_storedObjects.Remove(item);

        if (m_storedObjects.Count < inventoryMaxSize)
            isFull = false;
        if (m_storedObjects.Count == 0)
            isEmpty = true;

        OrderObjects();
    }

    private void OrderObjects()
    {
        // Create new slots if inventory is too small
        if(m_inventorySlotPositions.Count < m_storedObjects.Count)
        {
            // Missing implementation - clear inventoryslotpositions and calculate new ones depending on size & other parameters
            m_inventorySlotPositions.Clear();
            Vector3 position;
            for(int i = 0; i < m_storedObjects.Count; i++)
            {
                position = transform.position;
                position.x += radius * Mathf.Cos(i * Mathf.Deg2Rad * offsetInDegrees);
                position.y += radius * Mathf.Sin(i * Mathf.Deg2Rad * offsetInDegrees);

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
