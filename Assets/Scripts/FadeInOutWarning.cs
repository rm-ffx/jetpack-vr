using UnityEngine;
using System.Collections;

public class FadeInOutWarning : MonoBehaviour {

    private Texture m_warning;
    public float FadeSpeed = 1.0f;
    public Renderer Warning;

	// Use this for initialization
	void Start () {
        Warning = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void FadeIn()
    {
        
    }

    void FadeOut()
    {

    }
}
