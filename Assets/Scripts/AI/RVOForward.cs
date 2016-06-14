using UnityEngine;
using System.Collections;

public class RVOForward : MonoBehaviour
{
    public float speed;

    Pathfinding.RVO.RVOController controller;

	void Start ()
    {
        controller = GetComponent<Pathfinding.RVO.RVOController>();
	}
	
	void Update ()
    {
        controller.Move(transform.forward * speed);
	}
}
