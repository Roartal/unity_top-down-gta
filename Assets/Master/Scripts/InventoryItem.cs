using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem {

	public int itemID = -1;
	public Sprite icon;
	public Sprite iconDetails;
	public string name;
	public int meshID = -1;
	public AudioClip fireSound;
	[Tooltip("Rounds per minute!")]
	public float fireRate = 600;
	public int fireMode = 0;
	public int ammoPerMag = 0;
	public int ammoInMag = 0;
	public int bulletID = -1;
	public float reloadTime = 1;
	public float spread = 0.125f;
	public float spreadConstant = 0f;

	[HideInInspector]
	public GameObject pickupPrefab;
	public GameObject projectilePrefab;

	public float verticalRecoilMultiplier = 1;
	public float horizontalRecoilMultiplier = 1;

	public int bulletsPerShot = 1;

	public float damage = 1f;

	public bool melee = true;
	public float meleeDistance = 2.0f;
	public bool useDurability = false;
	public float durability = 50;
	public float maxDurability = 50;
	public float calcDurability;
	public bool ejectShells = true;
	public float ejectShellsDelay = 0;

	public Sprite GetImage()
	{
		return icon;
	}

	public Sprite GetImageDetails()
	{
		return iconDetails;
	}

	public string GetText()
	{
		return name;
	}

	public int GetID()
	{
		return bulletID;
	}

}
