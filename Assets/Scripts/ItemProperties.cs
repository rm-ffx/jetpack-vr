using UnityEngine;
using System.Collections;

/// <summary>
/// Stores information about what the player can do with specific objects
/// </summary>
public class ItemProperties : MonoBehaviour
{
    [HideInInspector]
    public bool IsInUse = false;
    //private bool m_oldIsInUse = false;

    [Tooltip("Gatherable items can be picked up by the player")]
    public bool Gatherable = false;
    [Tooltip("Keep in mind that Tossable objects need a non-zero drag to behave properly")]
    public bool Tossable = false;
    [Tooltip("Storable items can be put into the inventory")]
    public bool Storable = false;
    [Tooltip("The item's default material")]
    public Material Material;
    [Tooltip("The item's highlit material, which is used when the player can interact with it")]
    public Material HighlightMaterial;

    private MeshRenderer m_meshRenderer;

    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.material = Material;
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
            m_meshRenderer.material = HighlightMaterial;
        else
            m_meshRenderer.material = Material;
    }
}


