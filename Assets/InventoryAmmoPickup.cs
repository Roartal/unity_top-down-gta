using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAmmoPickup : MonoBehaviour {

	public int bulletID;
	public int amount = 16;
	public GlowObjectCmd glow;

	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<ThirdPersonVehicleInteraction> ()) {
			ThirdPersonInventory.staticInv.AddItemAmmo (bulletID,amount);
			if(glow) GlowController.UnregisterObject(glow);
			Destroy (gameObject);
			//gameObject.SetActive (false);
		}
	}
}
