using UnityEngine;
using System.Collections;

public class GadgetPickup : MonoBehaviour
{
    [Tooltip("The gadget that can be picked up by touching this object. The values of the gadget will be copied!")]
    public MonoBehaviour gadget;
    private bool m_isPickedUp = false;

    void Start()
    {
        gadget.enabled = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 10 && !m_isPickedUp)
        {
            m_isPickedUp = true;
            SteamVR_ControllerManager controllerManager = collider.transform.parent.GetComponent<SteamVR_ControllerManager>();
            controllerManager.left.GetComponent<GadgetSelector>().AddGadget(gadget);
            controllerManager.right.GetComponent<GadgetSelector>().AddGadget(gadget);
            Destroy(gameObject);
        }
    }
}
