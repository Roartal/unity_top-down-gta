using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonInventorySelector : MonoBehaviour {

	public ThirdPersonInventory inv;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			inv.SelectItem (0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			inv.SelectItem (1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			inv.SelectItem (2);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			inv.SelectItem (3);
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			inv.SelectItem (4);
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			inv.SelectItem (5);
		}
	}
}
