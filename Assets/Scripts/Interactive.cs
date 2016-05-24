using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour
{
    [Tooltip("Wheter or not this is a switch. Note that this may not work when cooldown is 0.")]
    public bool isSwitch = false;
    [Tooltip("Wheter or not this is active from start.")]
    public bool isActiveOnStart = false;
    [Tooltip("How long before the object can be used again.")]
    public float cooldown = 1.0f;

    [Tooltip("Wheter or not an item is needed to activate the object.")]
    public bool requiresItem = false;
    [Tooltip("The collider of the item required to activate the object.")]
    public Collider requiredItemCollider;

    [Tooltip("The script that gets executed. Script needs to have Activate and/or Deactivate function.")]
    public TriggerScript triggeredScript;

    [Tooltip("The material used when the object is active.")]
    public Material ActiveMaterial;
    [Tooltip("The material used when the object is deactivated.")]
    public Material DeactivatedMaterial;

    private MeshRenderer m_meshRenderer;

    private bool m_isActive = false;
    private float m_remainingCooldown = 0.0f;

	void Start ()
    {
        // Tag needed for the controller to be able to interact with it
        if (isSwitch)
            tag = "Interactive";

        m_isActive = isActiveOnStart;
        m_meshRenderer = GetComponent<MeshRenderer>();

        if (m_isActive)
            m_meshRenderer.material = ActiveMaterial;
        else
            m_meshRenderer.material = DeactivatedMaterial;

        if(triggeredScript == null)
            triggeredScript = GetComponent<TriggerScript>();
    }
	
	void Update ()
    {
        m_remainingCooldown -= Time.deltaTime;
        if (requiresItem)
            if (requiredItemCollider.enabled == false)
                Deactivate();
	}

    public void UseSwitch()
    {
        if(m_remainingCooldown <= 0.0f)
        {
            m_remainingCooldown = cooldown;
            if (m_isActive)
                Deactivate();
            else
                Activate();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider == requiredItemCollider)
            Activate();
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider == requiredItemCollider)
            Deactivate();
    }

    private void Activate()
    {
        m_isActive = true;
        m_meshRenderer.material = ActiveMaterial;
        if (triggeredScript != null)
            triggeredScript.Activate();
    }

    private void Deactivate()
    {
        m_isActive = false;
        m_meshRenderer.material = DeactivatedMaterial;
        if (triggeredScript != null)
            triggeredScript.Deactivate();
    }
}
