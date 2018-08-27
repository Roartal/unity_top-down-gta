using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSeatTrigger : MonoBehaviour {

	public VehicleParent vp;
	private ThirdPersonVehicleInteraction tpvi;
    Texture driverTexture;
    Renderer driverMesh;

	// Use this for initialization
	void Start () {
		vp = GetComponentInParent<VehicleParent> ();
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<ThirdPersonVehicleInteraction> ()) {
			if (tpvi == null)
				tpvi = col.GetComponent<ThirdPersonVehicleInteraction> ();
			if (tpvi.curVp == null)
				tpvi.curVp = vp;
		}
	}

    public void ToggleDriverMesh(bool on = true)
    {
        driverMesh.enabled = on;

    }

	void OnTriggerExit()
	{
		if (tpvi != null) {
			tpvi.curVp = null;
			tpvi = null;
		}
	}
}
