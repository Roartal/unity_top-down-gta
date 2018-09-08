using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSeatTrigger : MonoBehaviour {

	public VehicleParent vp;
	private ThirdPersonVehicleInteraction tpvi;
    Texture driverTexture;
    public Renderer driverMesh;

	// Use this for initialization
	void Start () {
		vp = GetComponentInParent<VehicleParent> ();
	}

    public void ToggleDriverMesh(bool on = true, Texture tex = null)
    {
        if (driverMesh)
        {
            driverMesh.enabled = on;
            driverMesh.material.SetTexture("_MainTex", tex);
        }

    }
}
