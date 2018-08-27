using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonVehicleInteraction : MonoBehaviour {

	public PlayerUI pUI;
	public VehicleParent curVp;

	public LayerMask layerMask;

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.E)) {
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, 1.48f, layerMask, QueryTriggerInteraction.Collide);

			hitColliders [0].SendMessage ("AddThis", SendMessageOptions.DontRequireReceiver);
		}
	}
}
