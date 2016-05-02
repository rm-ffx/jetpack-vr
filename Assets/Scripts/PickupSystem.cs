using UnityEngine;
using System.Collections;

// Attach to Controller
[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupSystem : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    GameObject handObject;
    public Inventory m_inventory { get; private set; }

    private GameObject m_otherDeviceGameObject;
    private PickupSystem m_otherDevicePickupSystem;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        m_inventory = null;
        handObject = null;

        m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().left;
        if (m_otherDeviceGameObject == gameObject)
            m_otherDeviceGameObject = transform.parent.GetComponent<SteamVR_ControllerManager>().right;

        m_otherDevicePickupSystem = m_otherDeviceGameObject.GetComponent<PickupSystem>();
    }

    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (handObject != null && device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0) && handObject.tag == "Item")
        {
            // Drop Item
            ItemProperties itemProperties = handObject.GetComponent<ItemProperties>();

            // If possible, store the item in the inventory
            if (m_inventory != null && itemProperties.Storable && !m_inventory.isFull)
            {
                //Debug.Log("Store item");
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
                    //rigidbody.velocity = origin.TransformVector(device.velocity);
                    //rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
                    rigidbody.velocity = origin.TransformVector(device.velocity) + transform.parent.GetComponent<Rigidbody>().velocity;
                    rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity); // + transform.parent.GetComponent<Rigidbody>().angularVelocity;
                }
                else
                {
                    rigidbody.velocity = device.velocity + transform.parent.GetComponent<Rigidbody>().velocity;
                    rigidbody.angularVelocity = device.angularVelocity; // + transform.parent.GetComponent<Rigidbody>().angularVelocity;
                }

                rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            }

            // Release object
            handObject.transform.parent = null;
            handObject = null;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            if (handObject == null && collider.tag == "Item" && collider.GetComponent<ItemProperties>().Gatherable)
            {
                if (m_inventory != null && !m_inventory.isEmpty)
                {
                    //Debug.Log("Withdraw Item");
                    PickupItem(m_inventory.WithdrawItem());
                }
                else
                {
                    //Debug.Log("Pick up item");
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
    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("TriggerEnter");
        if (collider.tag == "Inventory")
        {
            //Debug.Log("InventoryEnter");
            m_inventory = collider.GetComponent<Inventory>();
            m_inventory.OpenInventory();
        }
        //else
        //    Debug.Log("Collider Tag: " + collider.tag);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Inventory")
        {
            if(m_otherDevicePickupSystem.m_inventory != m_inventory)
            {
                m_inventory.CloseInventory();
            }
            //Debug.Log("InventoryExit");
            m_inventory = null;
        }
    }
}
