﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonUIHandler : MonoBehaviour {

	public Animator UI;

	public Image healthImage;
	public Image staminaImage;
	public Image staminaRestrictionImage;
	public Image bulletsImage;
	public Text ammoLeft;
	public Text ammoInMag;
	public Transform slotSelector;
	public static ThirdPersonUIHandler master;
	public CanvasGroup staminaWarning;

	public GameObject reticle;

	void Awake()
	{
		master = this;
	}

	public void UpdateHealth(float health, float maxHealth)
	{
		healthImage.fillAmount = health / maxHealth;
	}

    GameObject lastSelectedSlot;

	public void UpdateSlotSelectorPos(GameObject slot)
	{
        LeanTween.scale(lastSelectedSlot, Vector3.one, 0.1f);
        LeanTween.scale(slot, Vector3.one * 1.4f, 0.1f);
        slotSelector.position = slot.transform.position;
        lastSelectedSlot = slot;

    }

	public void UpdateStaminaRestriction(float s)
	{
		staminaRestrictionImage.fillAmount = s;
	}

	public void UpdateStaminaWarning(bool activate)
	{
		if (activate)
			staminaWarning.alpha = 1;
		else
			staminaWarning.alpha = 0;
	}

	public void UpdateAmmo(float mag, float left, float maxMag, bool melee = false, bool updateUI = true)
	{
		if(updateUI)
			UI.SetTrigger ("Fire");
		if (!melee) {
			ammoInMag.text = "" + mag;
			ammoLeft.text = "/ " + left;
			bulletsImage.fillAmount = mag / maxMag;
			if (mag == 0) {
				bulletsImage.fillAmount = 1;
				bulletsImage.color = Color.red;
			} else
				bulletsImage.color = Color.white;
		} else {
			ammoInMag.text = "";
			ammoLeft.text = "";
			bulletsImage.fillAmount = 0;
		}
	}
}
