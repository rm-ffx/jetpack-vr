using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Attach to Controller
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupSystem : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    GameObject handObject;
    public Inventory m_inventory { get; private set; }
    public bool m_isHandBusy { get; private set; }

    private GameObject m_otherDeviceGameObject;
    private PickupSystem m_otherDevicePickupSystem;
    
    private List<GameObject> m_itemsInRange;
    private GameObject m_closestObject;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        m_inventory = null;
        handObject = null;
        m_isHandBusy = false;
        m_itemsInRange = new List<GameObject>();

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if (m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        m_otherDevicePickupSystem = m_otherDeviceGameObject.GetComponent<PickupSystem>();
    }

    void Update()
    {
        if (handObject == null)
            FindAndHighlightClosestObject();
        else
        {
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            if (handObject != null && device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0) && handObject.tag == "Item")
            {
                ItemProperties itemProperties = handObject.GetComponent<ItemProperties>();

                // If possible, store the item in the inventory
                if (m_inventory != null && itemProperties.Storable && !m_inventory.isFull)
                {
                    itemProperties.Highlight(false);
                    m_inventory.StoreItem(handObject);
                }
                // If possible, toss the item
                else if (itemProperties.Tossable)
                {
                    //Debug.Log("Toss Object");
                    Rigidbody rigidbody = handObject.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = false;

                    var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                    if (origin != null)
                    {
                        rigidbody.velocity = origin.TransformVector(device.velocity);
                        rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
                        //rigidbody.AddForce(transform.parent.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                        //rigidbody.velocity = transform.parent.GetComponent<Rigidbody>().velocity;
                        //rigidbody.AddForce(origin.TransformVector(device.velocity), ForceMode.Impulse);
                        //rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity); // + transform.parent.GetComponent<Rigidbody>().angularVelocity;
                    }
                    else
                    {
                        rigidbody.velocity = device.velocity + transform.parent.GetComponent<Rigidbody>().velocity;
                        rigidbody.angularVelocity = device.angularVelocity; // + transform.parent.GetComponent<Rigidbody>().angularVelocity;
                    }

                    rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
                }

                // Release object
                if (handObject.transform.parent = transform)
                {
                    handObject.transform.parent = null;
                }
                itemProperties.IsInUse = false;
                itemProperties.Highlight(false);
                handObject = null;
                m_isHandBusy = false;
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            if (handObject == null)
            {
                if (m_inventory != null && !m_inventory.isEmpty)
                {
                    PickupItem(m_inventory.WithdrawItem(transform.position));
                }
                else if(m_closestObject != null && m_closestObject.tag == "Item" && m_closestObject.GetComponent<ItemProperties>().Gatherable)
                {
                    if (!m_closestObject.GetComponent<ItemProperties>().IsInUse)
                        PickupItem(m_closestObject);
                }
                else if (collider.tag == "Item" && collider.GetComponent<ItemProperties>().Gatherable)
                {
                    if(!collider.GetComponent<ItemProperties>().IsInUse)
                        PickupItem(collider.gameObject);
                }
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        handObject = item;
        handObject.transform.position = transform.position;
        handObject.transform.rotation = transform.rotation;
        handObject.GetComponent<Rigidbody>().isKinematic = true;
        handObject.transform.parent = transform;
        m_isHandBusy = true;

        ItemProperties itemProperties = handObject.GetComponent<ItemProperties>();
        itemProperties.IsInUse = true;
        itemProperties.Highlight(false);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Item")
            m_itemsInRange.Add(collider.gameObject);
        else if (collider.tag == "Inventory")
        {
            m_inventory = collider.GetComponent<Inventory>();
            m_inventory.OpenInventory();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Item")
        {
            collider.GetComponent<ItemProperties>().Highlight(false);
            m_itemsInRange.Remove(collider.gameObject);
        }
        else if (collider.tag == "Inventory")
        {
            foreach (GameObject go in m_inventory.m_storedObjects)
            {
                m_itemsInRange.Remove(go);
                go.GetComponent<ItemProperties>().Highlight(false);
            }

            if (m_otherDevicePickupSystem.m_inventory != m_inventory)
            {
                m_inventory.CloseInventory();
            }
            m_inventory = null;
        }
    }

    private void FindAndHighlightClosestObject()
    {
        m_closestObject = null;
        if (m_inventory != null && !m_inventory.isEmpty)
        {
            m_inventory.Highlight(transform.position);
        }
        else if (m_itemsInRange.Count == 0)
            return;
        else if (m_itemsInRange.Count == 1)
        {
            ItemProperties itemProperties = m_itemsInRange[0].GetComponent<ItemProperties>();
            if(itemProperties.IsInUse)
            {
                itemProperties.Highlight(false);
            }
            else
            {
                itemProperties.Highlight(true);
                m_closestObject = m_itemsInRange[0];
            }
        }
        else
        {
            float minMagnitude = float.MaxValue;
            GameObject closestObject = null;
            ItemProperties itemProperties = null;
            foreach (GameObject item in m_itemsInRange)
            {
                itemProperties = item.GetComponent<ItemProperties>();
                itemProperties.Highlight(false);
                if (itemProperties.IsInUse)
                    continue;

                float dist = (item.transform.position - transform.position).magnitude;
                if (dist < minMagnitude)
                {
                    minMagnitude = dist;
                    closestObject = item;
                }
            }
            if(closestObject != null)
            {
                closestObject.GetComponent<ItemProperties>().Highlight(true);
                m_closestObject = closestObject;
            }
        }
    }
}
