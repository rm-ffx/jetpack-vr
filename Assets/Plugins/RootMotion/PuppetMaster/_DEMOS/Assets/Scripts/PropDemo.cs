﻿using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	// Code example for picking up/dropping props.
	public class PropDemo : MonoBehaviour {

		[Tooltip("The Prop you wish to pick up.")] 
		public Prop prop;

		[Tooltip("The PropRoot to connect it to.")] 
		public PropRoot connectTo;

		[Tooltip("If true, the prop will be picked up when PuppetMaster initiates")]
		public bool pickUpOnStart;

		void Start() {
			if (pickUpOnStart) connectTo.currentProp = prop;
		}

		void Update () {
			if (Input.GetKeyDown(KeyCode.P)) {
				// Makes the prop root drop any existing props and pick up the newly assigned one.
				connectTo.currentProp = prop;
			}

			if (Input.GetKeyDown(KeyCode.X)) {
				// By setting the prop root's currentProp to null, the prop connected to it will be dropped.
				connectTo.currentProp = null;
			}
		}
	}
}
