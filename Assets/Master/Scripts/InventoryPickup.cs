using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickup : MonoBehaviour {

	public enum Rarity {normal, unique, rare, special, legendary}//The type of property

	public Rarity rarity;
	public GlowObjectCmd glow;
	[HideInInspector]
	public bool isActive = true;

	public int itemID = -1;
	public Sprite icon;
	public Sprite iconDetails;
	public string name;

	public float fireRate;
	public int fireMode = 1;
	public int ammoPerMag;
	public int ammoInMag;
	public int bulletID = -1;
	public int meshID = -1;
	public AudioClip fireSound;

	public bool melee = false;
	public bool useDurability = false;
	public float durability = 50;
	public float maxDurability = 50;

	public float damage = 1f;

	public float spread = 0.125f;
	public float spreadConstant = 0f;
	public float verticalRecoilMultiplier = 1;
	public float horizontalRecoilMultiplier = 1;
	public int bulletsPerShot = 1;
	public bool ejectShells = true;
	public bool mouseHoverActive = false;
	public GameObject interactionHover;


	public InventoryItem item = new InventoryItem();

	void Awake()
	{
		item.pickupPrefab = gameObject;
		switch (rarity)
		{
		case Rarity.normal:
			spread = spread * 1f;
			damage = damage * 1f;
			GetComponent<MeshRenderer> ().material.SetColor("_TintColor",new Color (1f, 1f, 1f,0.1f));
			glow.GlowColor = new Color (1f, 1f, 1f,0.1f); 
			break;

		case Rarity.unique:
			spread = spread * 0.96f;
			damage = damage * 1.05f;
			fireRate = fireRate * 1.01f;
			GetComponent<MeshRenderer> ().material.SetColor("_TintColor",new Color (0.333333f, 1f, 0.2431372f,0.1f));
			glow.GlowColor = new Color (0.333333f, 1f, 0.2431372f,0.1f); 
			break;

		case Rarity.rare:
			spread = spread * 0.82f;
			damage = damage * 1.1f;
			fireRate = fireRate * 1.02f;
			GetComponent<MeshRenderer> ().material.SetColor("_TintColor",new Color (0, 0.584314f, 1f,0.1f));
			glow.GlowColor = new Color (0, 0.584314f, 1f,0.1f);
			break;

		case Rarity.special:
			spread = spread * 0.77f;
			damage = damage * 1.16f;
			fireRate = fireRate * 1.04f;
			GetComponent<MeshRenderer> ().material.SetColor("_TintColor",new Color (1f, 0.63529412f, 0.2431372f,0.1f));
			glow.GlowColor = new Color (1f, 0.63529412f, 0.2431372f,0.1f);
			break;

		case Rarity.legendary:
			spread = spread * 0.7f;
			damage = damage * 1.24f;
			fireRate = fireRate * 1.08f;
			GetComponent<MeshRenderer> ().material.SetColor("_TintColor",new Color (1f, 0.2431372f, 0.9803922f,0.1f));
			glow.GlowColor = new Color (1f, 0.2431372f, 0.9803922f,0.1f);
			break;
		}
		item.itemID = itemID;
		item.icon = icon;
		item.name = name;
		item.fireMode = fireMode;
		item.fireRate = fireRate;
		item.ammoPerMag = ammoPerMag;
		item.ammoInMag = ammoInMag;
		item.bulletID = bulletID;
		item.meshID = meshID;
		item.fireSound = fireSound;
		item.melee = melee;
		item.damage = damage;
		item.iconDetails = iconDetails;
		item.spread = spread;
		item.spreadConstant = spreadConstant;
		item.bulletsPerShot = bulletsPerShot;
		item.ejectShells = ejectShells;
		item.horizontalRecoilMultiplier = horizontalRecoilMultiplier;
		item.verticalRecoilMultiplier = verticalRecoilMultiplier;

		item.useDurability = useDurability;
		item.maxDurability = maxDurability;
		item.durability = durability;

	}

	public void AddThis()
	{
		ThirdPersonInventory.staticInv.AddItem (item);
	}

	void OnTriggerEnter(Collider col)
	{
		if (isActive && col.GetComponent<ThirdPersonVehicleInteraction> ()) {
			AddThis ();
		}
	}

	void Update()
	{
		if (mouseHoverActive) {
			if (Input.GetKeyDown (KeyCode.E)) {
				AddThis ();
			}
		}
	}

	void OnMouseOver()
	{
		interactionHover.SetActive (true);
		mouseHoverActive = true;
	}

	void OnMouseExit()
	{
		interactionHover.SetActive (false);
		mouseHoverActive = false;
	}
}
