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
    public Image warning;
	VehicleParent vp;
	Motor engine;
	Transmission trans;
	GearboxTransmission gearbox;
	ContinuousTransmission varTrans;
    //VehicleUI
    public GameObject vehicleUI;
    Vector3 vehicleUIrestpos;
    Vector3 vehicleUIactivepos;
    //InventoryAmmoUI
    public GameObject inventoryAmmoUI;
    Vector3 inventoryAmmoUIrestpos;
    Vector3 inventoryAmmoUIactivepos;

    public GameObject newVehicle;
	public GameObject playerController;
    public SkinnedMeshRenderer playerMesh;
	public UnityStandardAssets.Utility.SmoothFollow cam;
	public bool inVehicle = true;
	public ThirdPersonRagdoll tpRagdoll;

    void Awake()
    {
        vehicleUIactivepos = vehicleUI.transform.position;
        vehicleUIrestpos = vehicleUIactivepos - Vector3.right * 100f;
        vehicleUI.transform.position = vehicleUIrestpos;

        inventoryAmmoUIactivepos = inventoryAmmoUI.transform.position;
        inventoryAmmoUIrestpos = inventoryAmmoUIactivepos + Vector3.right * 100f;
    }
	
	// Update is called once per frame
	void Update () {

		if (vp && rpmMeter != null)
		{
			//vp.trans.automatic = autoShiftToggle.isOn;
			speedText.text = (vp.velMag * 3.5f).ToString("0") + " KPH";

			if (vp.trans)
			{
				if (vp.gearbox)
				{
					gearText.text = (vp.gearbox.currentGear == 0 ? "R" : (vp.gearbox.currentGear == 1 ? "N" : (vp.gearbox.currentGear - 1).ToString()));
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

                if(vp.engine.health < 0.6f)
                {
                    if(vp.engine.health < 0.15f)
                    {
                        warning.color = Color.red;
                    }
                    else
                    {
                        warning.color = Color.yellow;
                    }
                }
                else
                {
                    warning.color = Color.clear;
                }
			}
		}
	}

	public void SmurfPlayer(Vector3 pos)
    {
        tpRagdoll.DisableRagdoll();
        tpRagdoll.enabled = false;
        LeanTween.move(playerController, pos, 0.2f);
        LeanTween.scale(playerController, Vector3.one * 0.01f, 0.2f);
        Invoke("DisablePlayer", 0.2f);
    }
    public void UnsmurfPlayer()
    {
        EnablePlayer();
        tpRagdoll.enabled = true;
        tpRagdoll.DisableRagdoll();
        LeanTween.scale(tpRagdoll.root, Vector3.one * 100f, 0.1f);
        LeanTween.scale(playerController, Vector3.one, 0.1f);
        playerController.transform.position = vp.gameObject.transform.position - vp.gameObject.transform.right;
    }

    void DisablePlayer()
    {
        tpRagdoll.DisableRagdoll();
        playerController.SetActive(false);
    }

    void EnablePlayer()
    {
        tpRagdoll.DisableRagdoll();
        playerController.SetActive(true);
    }

    public void ToggleVehicle(VehicleParent newVP)
    {
        if (newVP == null)
        {
            vp.driverSeat.ToggleDriverMesh(false);
            vp.ExitVehicle();
            cam.target = playerController.transform;
            UnsmurfPlayer();
            cam.vel = tpRagdoll.mainRB;
            cam.followVel = true;
            vp = null;
            inVehicle = false;
            LeanTween.move(vehicleUI, vehicleUIrestpos, 0.1f);
            LeanTween.move(inventoryAmmoUI, inventoryAmmoUIactivepos, 0.1f);
        }
        else {
            vp = newVP;
            vp.driverSeat.ToggleDriverMesh(true, playerMesh.material.mainTexture);
            vp.EnterVehicle();
            SmurfPlayer(vp.driverSeat.transform.position);
            gearbox = vp.gameObject.GetComponentInChildren<GearboxTransmission>();
            engine = vp.gameObject.GetComponentInChildren<Motor>();
            trans = vp.gameObject.GetComponentInChildren<Transmission>();
            //			trans.automatic = autoShiftToggle.isOn;
            cam.target = vp.gameObject.transform;
            cam.vel = vp.rb;
            cam.followVel = true;
            inVehicle = true;
            LeanTween.move(vehicleUI, vehicleUIactivepos, 0.1f);
            LeanTween.move(inventoryAmmoUI, inventoryAmmoUIrestpos, 0.1f);
        }
        
    }
}
