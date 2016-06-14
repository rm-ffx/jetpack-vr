using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Interactive : MonoBehaviour
{
    [Tooltip("Whether or not this is a switch. Note that this may not work when cooldown is 0.")]
    public bool isSwitch = false;
    [Tooltip("Whether or not this is active from start.")]
    public bool isActiveOnStart = false;
    [Tooltip("Whether or not the object will be destroyed after interacting with it.")]
    public bool destroyOnUse = false;
    [Tooltip("How long before the object can be used again.")]
    public float cooldown = 1.0f;

    [Tooltip("Whether or not an item is needed to activate the object.")]
    public bool requiresItem = false;
    [Tooltip("The list of the items colliders required to activate the object.")]
    public List<Collider> requiredItemsCollider;
    private bool[] detectedItems;

    [Tooltip("The script that gets executed. Script needs to have Activate and/or Deactivate function.")]
    public TriggerScript triggeredScript;

    [Tooltip("The material used when the object is active.")]
    public Material activeMaterial;
    [Tooltip("The material used when the object is deactivated.")]
    public Material deactivatedMaterial;

    private MeshRenderer m_meshRenderer;

    private bool m_isActive = false;
    private float m_remainingCooldown = 0.0f;

	void Start ()
    {
        // Tag needed for the controller to be able to interact with it
        if (isSwitch)
            tag = "Interactive";

        detectedItems = new bool[requiredItemsCollider.Count];
        detectedItems = Enumerable.Repeat(false, requiredItemsCollider.Count).ToArray();

        m_isActive = isActiveOnStart;
        m_meshRenderer = GetComponent<MeshRenderer>();

        if (m_isActive)
            m_meshRenderer.material = activeMaterial;
        else
            m_meshRenderer.material = deactivatedMaterial;

        if(triggeredScript == null)
            triggeredScript = GetComponent<TriggerScript>();
    }
	
	void Update ()
    {
        m_remainingCooldown -= Time.deltaTime;
        if (requiresItem)
        {
            for(int i = 0; i < requiredItemsCollider.Count; i++)
            {
                if (requiredItemsCollider[i].enabled == false)
                {
                    detectedItems[i] = false;
                    if(m_isActive)
                        Deactivate();
                }
            }
        }
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
        bool change = false;
        for (int i = 0; i < requiredItemsCollider.Count; i++)
        {
            if(collider == requiredItemsCollider[i])
            {
                detectedItems[i] = true;
                change = true;
            }
        }
        if(change)
        {
            bool allItemsDetected = true;
            for (int i = 0; i < detectedItems.Length; i++)
            {
                if (detectedItems[i] == false)
                {
                    allItemsDetected = false;
                    break;
                }
            }
            if(allItemsDetected)
                Activate();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        bool change = false;
        for (int i = 0; i < requiredItemsCollider.Count; i++)
        {
            if (collider == requiredItemsCollider[i])
            {
                detectedItems[i] = false;
                change = true;
            }
        }
        if (change)
        {
            bool allItemsDetected = true;
            for (int i = 0; i < detectedItems.Length; i++)
            {
                if (detectedItems[i] == false)
                {
                    allItemsDetected = false;
                    break;
                }
            }
            if (!allItemsDetected)
                Deactivate();
        }
    }

    private void Activate()
    {
        m_isActive = true;
        m_meshRenderer.material = activeMaterial;

        if (triggeredScript != null)
            triggeredScript.Activate();

        if (destroyOnUse)
            Destroy(gameObject);
    }

    private void Deactivate()
    {
        m_isActive = false;
        m_meshRenderer.material = deactivatedMaterial;

        if (triggeredScript != null)
            triggeredScript.Deactivate();

        if (destroyOnUse)
            Destroy(gameObject);
    }
}
