using UnityEngine;
using System.Collections;

public class RVOForward : MonoBehaviour
{
    public float speed;

    Pathfinding.RVO.RVOController controller;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponent<Pathfinding.RVO.RVOController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        controller.Move(transform.forward * speed);
	}
}
