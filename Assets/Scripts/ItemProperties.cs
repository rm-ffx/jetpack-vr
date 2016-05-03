using UnityEngine;
using System.Collections;

public class ItemProperties : MonoBehaviour
{
    [HideInInspector]
    public bool IsInUse = false;
    //private bool m_oldIsInUse = false;

    public bool Gatherable = false;
    public bool Tossable = false;
    public bool Storable = false;
    public Material Material;
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


