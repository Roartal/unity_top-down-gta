using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public ThirdPersonVehicleInteraction tpvi;
	public Toggle autoShiftToggle;
	public Toggle assistToggle;
	public Text speedText;
	public Text gearText;
	public Image rpmMeter;
	public Slider boostMeter;
	VehicleParent vp;
	Motor engine;
	Transmission trans;
	GearboxTransmission gearbox;
	ContinuousTransmission varTrans;

	public GameObject newVehicle;
	public GameObject playerController;
	public UnityStandardAssets.Utility.SmoothFollow cam;
	public bool inVehicle = true;
	public ThirdPersonRagdoll tpRagdoll;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.E) && !tpRagdoll.ragdolling) {
			ToggleVehicle ();
		}

		if (vp && rpmMeter != null)
		{
			//vp.trans.automatic = autoShiftToggle.isOn;
			speedText.text = (vp.velMag * 3.6f).ToString("0") + " KPH";

			if (vp.trans)
			{
				if (vp.gearbox)
				{
					gearText.text = "Gear: " + (vp.gearbox.currentGear == 0 ? "R" : (vp.gearbox.currentGear == 1 ? "N" : (vp.gearbox.currentGear - 1).ToString()));
				}
				else if (varTrans)
				{
					gearText.text = "Ratio: " + varTrans.currentRatio.ToString("0.00");
				}
			}

			if (vp.engine)
			{
				if (vp.engine.ignition) {
					//rpmMeter.fillAmount = Mathf.Lerp(rpmMeter.fillAmount, engine.targetPitch + 0.1f, Time.deltaTime * 2);
					rpmMeter.fillAmount = vp.engine.targetPitch;
				}
				else
					rpmMeter.fillAmount = 0;

				if (vp.engine.maxBoost > 0)
				{
					//boostMeter.value = vp.engine.boost / vp.engine.maxBoost;
				}
			}
		}
	}

	public void CheckPickup()
	{
		
	}

	public void ToggleVehicle()
	{
		if (inVehicle) {
			vp.ExitVehicle ();
			playerController.SetActive (true);
			cam.target = playerController.transform;
			playerController.transform.position = vp.gameObject.transform.position - vp.gameObject.transform.right;
			cam.vel = null;
			vp = null;
			inVehicle = false;
		} else if (tpvi.curVp != null) {
			vp = tpvi.curVp;
			vp.EnterVehicle ();
			gearbox = vp.gameObject.GetComponentInChildren<GearboxTransmission> ();
			engine = vp.gameObject.GetComponentInChildren<Motor> ();
			trans = vp.gameObject.GetComponentInChildren<Transmission> ();
//			trans.automatic = autoShiftToggle.isOn;
			playerController.SetActive (false);
			cam.target = vp.gameObject.transform;
			cam.vel = vp.gameObject.GetComponent<Rigidbody> ();
			inVehicle = true;
		} else
			print ("Found no vehicle within your reach!");
	}
}
