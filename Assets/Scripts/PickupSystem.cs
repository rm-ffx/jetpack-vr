using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles picking up objects, storing and tossing them
/// </summary>
// Attach to Controller
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupSystem : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    GameObject handObject;

    [Tooltip("Connect an inventory here. It will be used as the player's portable inventory.")]
    public Inventory portableInventory;
    private bool m_inventoryOpen;
    public Inventory m_inventory { get; private set; }
    public bool m_isHandBusy { get; private set; }

    private GadgetSelector m_gadgetSelector;
    private bool m_gadgetSelectorOpen;

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
        m_inventoryOpen = false;
        m_gadgetSelectorOpen = false;

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if (m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        m_otherDevicePickupSystem = m_otherDeviceGameObject.GetComponent<PickupSystem>();

        m_gadgetSelector = GetComponent<GadgetSelector>();
    }

    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (handObject == null)
        {
            if(!m_isHandBusy)
                FindAndHighlightClosestObject();

            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
                OpenGadgetSelector();
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0))
                CloseGadgetSelector();
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
                EnableInventory();
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu) && m_inventoryOpen)
                DisableInventory();
        }
        else
        {
            if (handObject != null && device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0) && handObject.tag == "Item")
            {
                ItemProperties itemProperties = handObject.GetComponent<ItemProperties>();

                // If possible, store the item in the inventory
                if (m_inventory != null && itemProperties.storable && !m_inventory.isFull)
                {
                    m_inventory.StoreItem(handObject);
                }
                // If possible, toss the item
                else if (itemProperties.tossable)
                {
                    Rigidbody handObjectRigidbody = handObject.GetComponent<Rigidbody>();
                    Rigidbody playerRigidbody = transform.parent.GetComponent<Rigidbody>();
                    handObjectRigidbody.isKinematic = false;

                    var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                    if (origin != null)
                    {
                        handObjectRigidbody.velocity = playerRigidbody.velocity + origin.TransformVector(device.velocity);
                        handObjectRigidbody.angularVelocity = playerRigidbody.angularVelocity + origin.TransformVector(device.angularVelocity);
                    }
                    else
                    {
                        handObjectRigidbody.velocity = playerRigidbody.velocity + device.velocity;
                        handObjectRigidbody.angularVelocity = playerRigidbody.angularVelocity + device.angularVelocity; 
                    }

                    handObjectRigidbody.maxAngularVelocity = handObjectRigidbody.angularVelocity.magnitude;
                }

                // Release object
                if (handObject.transform.parent == transform)
                {
                    handObject.transform.parent = null;
                }
                itemProperties.isInUse = false;
                itemProperties.Highlight(false);
                handObject.GetComponent<Collider>().enabled = true;
                handObject = null;
                m_isHandBusy = false;
                m_closestObject = null;
                FindAndHighlightClosestObject();
                //m_itemsInRange.Clear();
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            if (handObject == null && !m_isHandBusy)
            {
                if (m_inventory != null && !m_inventory.isEmpty)
                    PickupItem(m_inventory.WithdrawItem(transform.position));
                else if(m_closestObject != null && m_closestObject.tag == "Item" && m_closestObject.GetComponent<ItemProperties>().gatherable && !m_closestObject.GetComponent<ItemProperties>().isInUse)
                    PickupItem(m_closestObject);
                else if (collider.tag == "Item" && collider.GetComponent<ItemProperties>().gatherable && !collider.GetComponent<ItemProperties>().isInUse)
                    PickupItem(collider.gameObject);
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        handObject = item;
        handObject.transform.position = transform.position;
        handObject.transform.rotation = transform.rotation;
        handObject.GetComponent<Collider>().enabled = false;
        handObject.GetComponent<Rigidbody>().isKinematic = true;
        handObject.transform.parent = transform;
        m_isHandBusy = true;

        ItemProperties itemProperties = handObject.GetComponent<ItemProperties>();
        itemProperties.isInUse = true;
        itemProperties.Highlight(false);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Item")
            m_itemsInRange.Add(collider.gameObject);
        else if (collider.tag == "Inventory" && !m_inventoryOpen)
            m_inventory = collider.GetComponent<Inventory>();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Item")
        {
            collider.GetComponent<ItemProperties>().Highlight(false);
            m_itemsInRange.Remove(collider.gameObject);
        }
        else if (collider.tag == "Inventory" && !m_inventoryOpen)
        {
            foreach (GameObject go in m_inventory.m_storedObjects)
            {
                m_itemsInRange.Remove(go);
                go.GetComponent<ItemProperties>().Highlight(false);
            }

            //if (m_otherDevicePickupSystem.m_inventory != m_inventory)
            //{
            //    m_inventory.CloseInventory();
            //}
            m_inventory = null;
        }
    }

    private void FindAndHighlightClosestObject()
    {
        m_closestObject = null;
        if(m_gadgetSelectorOpen)
        {
            m_gadgetSelector.Highlight(transform.position);
        }
        else if (m_inventory != null && !m_inventory.isEmpty)
        {
            m_inventory.Highlight(transform.position);
        }
        else if (m_itemsInRange.Count == 0)
            return;
        else if (m_itemsInRange.Count == 1)
        {
            ItemProperties itemProperties = m_itemsInRange[0].GetComponent<ItemProperties>();
            if(itemProperties.isInUse)
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
                if (itemProperties.isInUse)
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
                m_closestObject = closestObject;
                m_closestObject.GetComponent<ItemProperties>().Highlight(true);
            }
        }
    }

    private void EnableInventory()
    {
        if (portableInventory.gameObject.activeInHierarchy)
            m_otherDevicePickupSystem.DisableInventory();

        portableInventory.gameObject.SetActive(true);
        portableInventory.transform.position = transform.position;
        portableInventory.transform.rotation = transform.rotation;
        portableInventory.transform.parent = transform;
        portableInventory.OpenInventory();

        GetComponent<Collider>().enabled = false;
        m_inventoryOpen = true;
        m_isHandBusy = true;
    }

    private void DisableInventory()
    {
        if (m_inventoryOpen)
        {
            m_itemsInRange.Clear();
        }
        portableInventory.CloseInventory();
        m_otherDevicePickupSystem.RemoteInventoryClose();
        portableInventory.gameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;
        m_inventoryOpen = false;
        m_isHandBusy = false;
    }

    public void RemoteInventoryClose()
    {
        if (m_inventory != null)
        {
            foreach (GameObject go in m_inventory.m_storedObjects)
            {
                m_itemsInRange.Remove(go);
                go.GetComponent<ItemProperties>().Highlight(false);
            }
        }

        m_inventoryOpen = false;
        m_inventory = null;
        m_closestObject = null;
        FindAndHighlightClosestObject();
    }

    private void OpenGadgetSelector()
    {
        m_gadgetSelectorOpen = true;
        m_gadgetSelector.OpenGadgetSelector();
    }

    private void CloseGadgetSelector()
    {
        m_gadgetSelectorOpen = false;
        m_gadgetSelector.CloseGadgetSelector(transform.position);
    }
}
