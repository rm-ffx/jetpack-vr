using UnityEngine;
using System.Collections;

public class TriggerActivatePortal : TriggerScript
{
    [Tooltip("The collider of the portal to activate.")]
    public Collider portalCollider;
    [Tooltip("The material used when the object is active.")]
    public Material activeMaterial;
    [Tooltip("The material used when the object is deactivated.")]
    public Material deactivatedMaterial;

    public override void Activate()
    {
        if(!m_isActive)
        {
            m_isActive = true;
            portalCollider.enabled = true;
            GetComponent<MeshRenderer>().material = activeMaterial;
        }
    }

    public override void Deactivate()
    {
        if(m_isActive)
        {
            m_isActive = false;
            portalCollider.enabled = false;
            GetComponent<MeshRenderer>().material = deactivatedMaterial;
        }
    }
}
