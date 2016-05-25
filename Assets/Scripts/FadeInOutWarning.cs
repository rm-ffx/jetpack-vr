using UnityEngine;
using System.Collections;

public class FadeInOutWarning : MonoBehaviour
{

    private Texture m_warning;
    private MeshRenderer Warning;
    private bool m_isAlpha = false;
    private bool m_switchFade = true;

    public Levelborder Border;
    public float FadeSpeed = 0.01f;


    // Use this for initialization
    void Start()
    {
        Warning = GetComponent<MeshRenderer>();
    }

    IEnumerator FadeOut(float from, float to)
    {
        for (float f = from; f >= to; f -= FadeSpeed)
        {
            Color c = Warning.material.color;
            c.a = f;
            Warning.material.color = c;
            yield return null;
        }
        m_isAlpha = true;
        m_switchFade = true;
    }

    IEnumerator FadeIn(float from, float to)
    {
        for (float f = from; f < to; f += FadeSpeed)
        {
            Color c = Warning.material.color;
            c.a = f;
            Warning.material.color = c;
            yield return null;
        }
        m_isAlpha = false;
        m_switchFade = true;
    }

    // Update is called once per frame
    //bool m_switchFade = true;
    void Update()
    {
        if (Border.WarningOn && m_switchFade)
        {
            m_switchFade = false;
            if (!m_isAlpha)
            {
                StartCoroutine(FadeIn(0.0f, 0.75f));
                Debug.Log("FadeOut");
            }
            if (m_isAlpha)
            {
                StartCoroutine(FadeOut(0.75f, 0.0f));
                Debug.Log("FadeIn");
            }
        }
        if (!Border.WarningOn)
        {
            Color c = Warning.material.color;
            c.a = 0.0f;
            Warning.material.color = c;
        }
    }
}
