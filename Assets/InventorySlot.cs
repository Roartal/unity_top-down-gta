using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IInventorySlot {
	[HideInInspector]
	public Transform player;
	public InventoryItem curItem = new InventoryItem ();
	public Image iconHolder;
	public Image ammoTypeHolder;
	public Text ammoText;
	public GameObject durabilityParent;
	public Image durabilityHolder;
	public int bulletID;

	public Sprite defaultIcon;
	public Sprite defaultAmmoIcon;

	public int slotID;

	public InventoryItem GetItem()
	{
		return curItem;
	}

	void Awake()
	{
		bulletID = -1;
		iconHolder.sprite = defaultIcon;
		ammoTypeHolder.sprite = defaultAmmoIcon;
	}

	public void SetItem(InventoryItem newItem)
	{
		curItem = newItem;
		iconHolder.sprite = curItem.GetImage ();
		ammoTypeHolder.sprite = curItem.GetImageDetails ();
		bulletID = curItem.bulletID;
		durabilityParent.SetActive (curItem.melee);
		if (curItem.melee) {
			ammoText.text = "";
		}
		if (curItem.pickupPrefab)
			StorePickup ();
	}

	public void RemoveItem()
	{
		if (curItem.pickupPrefab)
			DisplayPickup ();
		curItem = new InventoryItem ();
		iconHolder.sprite = defaultIcon;
		ammoTypeHolder.sprite = defaultAmmoIcon;
		bulletID = -1;
	}

	public void DisplayPickup()
	{
		curItem.pickupPrefab.transform.position = player.position + Vector3.up * 0.02f;
		curItem.pickupPrefab.SetActive (true);
	}

	public void StorePickup()
	{
		curItem.pickupPrefab.transform.position = transform.position + Vector3.up * 500f;
		curItem.pickupPrefab.SetActive (false);
	}

	public void RequestSelection()
	{
		ThirdPersonInventory.staticInv.SelectItem (slotID);
	}

	public void RequestDrop()
	{
		ThirdPersonInventory.staticInv.RemoveItem (slotID);
	}

	public void FixedUpdate()
	{
		if (curItem.melee && curItem.itemID != -1) {
			durabilityHolder.fillAmount = curItem.calcDurability;
			return;
		}
		switch (bulletID) {
		case -1:
			ammoText.text = "";
			break;
		case 0:
			ammoText.text = ThirdPersonInventory.staticInv.smallAmmoCount.ToString ();
			break;
		case 1:
			ammoText.text = ThirdPersonInventory.staticInv.mediumAmmoCount.ToString ();
			break;
		case 2:
			ammoText.text = ThirdPersonInventory.staticInv.heavyAmmoCount.ToString ();
			break;
		case 3:
			ammoText.text = ThirdPersonInventory.staticInv.shotgunAmmoCount.ToString ();
			break;
		case 4:
			break;
		case 5:
			break;
		}
	}
}
