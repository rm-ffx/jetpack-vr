using UnityEngine;
using System.Collections;

/// <summary>
/// Stores information about what the player can do with specific objects
/// </summary>
public class ItemProperties : MonoBehaviour
{
    [HideInInspector]
    public bool isInUse = false;
    //private bool m_oldIsInUse = false;

    [Tooltip("Gatherable items can be picked up by the player.")]
    public bool gatherable = false;
    [Tooltip("Keep in mind that Tossable objects need a non-zero drag to behave properly.")]
    public bool tossable = false;
    [Tooltip("Storable items can be put into the inventory.")]
    public bool storable = false;
    [Tooltip("The item's default material.")]
    public Material material;
    [Tooltip("The item's highlit material, which is used when the player can interact with it.")]
    public Material highlightMaterial;

    private MeshRenderer m_meshRenderer;

    void Start()
    {
        // To make manual tagging unnecessary
        if (tag == "Untagged")
            tag = "Item";
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.material = material;
    }

    //void Update()
    //{
    //    if (IsInUse != m_oldIsInUse)
    //    {
    //        if(IsInUse)
    //            Highlight(false);

    //        m_oldIsInUse = IsInUse;
    //    }
    //}

    public void Highlight(bool highlightOn)
    {
        if (highlightOn)
            m_meshRenderer.material = highlightMaterial;
        else
            m_meshRenderer.material = material;
    }
}


