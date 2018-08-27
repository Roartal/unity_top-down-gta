using UnityEngine;
using System.Collections;
[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Basic Input", 0)]

//Class for setting the input with the input manager
public class BasicInput : MonoBehaviour
{
	VehicleParent vp;
	LightController lg;
	Motor engine;
	public string accelAxis;
	public string brakeAxis;
	public string steerAxis;
	public string ebrakeAxis;
	public string boostButton;
	public string upshiftButton;
	public string downshiftButton;
	public string pitchAxis;
	public string yawAxis;
	public string rollAxis;

	public string ignitionButton;
	public float ignitionTime;
	public float maxIgnitionTime = 0.5f;
	public string lightsButton;
	public string hornButton;

	public string exitVehicleButton;

	void Start()
	{
		vp = GetComponent<VehicleParent>();
		lg = GetComponent<LightController> ();
		engine = vp.engine;
	}

	void Update()
	{
		if (Input.GetButtonDown(upshiftButton))
		{
            Debug.Log("upshift");
			vp.PressUpshift();
        }

		if (Input.GetButtonDown(downshiftButton))
		{
            Debug.Log("downshift");
            vp.PressDownshift();
		}

		if (Input.GetButtonDown(lightsButton))
		{
			lg.headlightsOn = !lg.headlightsOn;
		}

		if (Input.GetButton (ignitionButton) && ignitionTime < maxIgnitionTime && !engine.ignition) {
			ignitionTime += Time.deltaTime;
		} else if (ignitionTime >= maxIgnitionTime) {
			ignitionTime = 0;
			engine.ignition = true;
		} else if (Input.GetButtonDown (ignitionButton) && engine.ignition) {
			engine.ignition = false;
		} else if (!Input.GetButton (ignitionButton)) {
			ignitionTime = 0;
		}

		if (Input.GetButton(hornButton)) {
			if(!vp.horn.isPlaying) {
				vp.horn.loop = true;
				if(vp.horn)
					vp.horn.Play();
			}
		} else {
			if(vp.horn)
				vp.horn.Stop();
		}
	}

	void FixedUpdate()
	{
		//Get constant inputs
		if (!string.IsNullOrEmpty(accelAxis))
		{
			vp.SetAccel(Input.GetAxis(accelAxis));
		}

		if (!string.IsNullOrEmpty(brakeAxis))
		{
			vp.SetBrake(Input.GetAxis(brakeAxis));
		}

		if (!string.IsNullOrEmpty(steerAxis))
		{
			vp.SetSteer(Input.GetAxis(steerAxis));
		}

		if (!string.IsNullOrEmpty(ebrakeAxis))
		{
			vp.SetEbrake(Input.GetAxis(ebrakeAxis));
		}

		if (!string.IsNullOrEmpty(boostButton))
		{
			vp.SetBoost(Input.GetButton(boostButton));
		}
		
		if (!string.IsNullOrEmpty(pitchAxis))
		{
			vp.SetPitch(Input.GetAxis(pitchAxis));
		}

		if (!string.IsNullOrEmpty(yawAxis))
		{
			vp.SetYaw(Input.GetAxis(yawAxis));
		}

		if (!string.IsNullOrEmpty(rollAxis))
		{
			vp.SetRoll(Input.GetAxis(rollAxis));
		}

		if (!string.IsNullOrEmpty(upshiftButton))
		{
			vp.SetUpshift(Input.GetAxis(upshiftButton));
		}
		
		if (!string.IsNullOrEmpty(downshiftButton))
		{
			vp.SetDownshift(Input.GetAxis(downshiftButton));
		}
	}
}
