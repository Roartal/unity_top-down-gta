using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPickable {
	void PickUp ();
}

/*public interface IInventoryItem {
	Sprite GetImage ();
	string GetText();
	int GetID();
}*/

public interface IInventorySlot {
	InventoryItem GetItem();
	void SetItem(InventoryItem newItem);
}

public class ThirdPersonInventory : MonoBehaviour {

	public ThirdPersonAimAtMouse tpAimScript;

	public int smallAmmoCount = 0;
	public int mediumAmmoCount = 0;
	public int heavyAmmoCount = 0;
	public int shotgunAmmoCount = 0;

	public int invSize = 4;
	public int totalItemCount = 0;
	public int invSelected = -1;
	public GameObject slotPrefab;
	public Transform layoutGroup;

	public InventorySlot[] inv;
	public ThirdPersonCharacterMotor tpMotor;

	public static ThirdPersonInventory staticInv;

	public void Start()
	{
		staticInv = this;
		inv = new InventorySlot[invSize];
		GenerateSlots ();
		SelectItem (0);
	}

	public void GenerateSlots()
	{
		for (int i = 0; i < invSize; i++) {
			GameObject c = Instantiate (slotPrefab, layoutGroup);
			inv [i] = c.GetComponent<InventorySlot> ();
			inv [i].slotID = i;
			inv [i].player = transform;
		}
	}

	public void AddItem(InventoryItem item, int index = -1) {
		if (index == -1) {
			index = this.GetFreeInvIndex ();
		} else {
			if (this.inv [index] != null)
				throw new UnityException ("Store index is not empty!");
		}

		this.inv [index].SetItem (item);
		totalItemCount++;
		tpMotor.UpdateFatigue (totalItemCount);
		if(index == invSelected)
			SelectItem (invSelected);
	}

	public void RemoveItem(int index)
	{
		if (this.inv [index] != null && inv[index].curItem.itemID != -1) {
			inv [index].RemoveItem ();
			totalItemCount--;
			tpMotor.UpdateFatigue (totalItemCount);
			if(index == invSelected)
				SelectItem (invSelected);
		}
	}

	public void AddItemAmmo (int bulletID, int amount)
	{
		switch (bulletID) {
		case 0:
			smallAmmoCount += amount;
			break;
		case 1:
			mediumAmmoCount += amount;
			break;
		case 2:
			heavyAmmoCount += amount;
			break;
		case 3:
			shotgunAmmoCount += amount;
			break;
		}
	}
		
	private int GetFreeInvIndex() {
		for (int i = 0; i < this.inv.GetLength (0); i++) {
			if (this.inv [i].curItem.GetID() == -1 || this.inv[i].curItem == null)
				return i;
		}
		throw new InventoryFullException ("NoFreeIndex");
	}

	public void SelectItem (int i)
	{
	//	if (invSelected != i){
		bool updateUI = true;
		if (invSelected == i)
			updateUI = false;
		tpAimScript.SwitchWeapon (inv [i].curItem, updateUI);
		invSelected = i;
        for (int b = 0; b < this.inv.GetLength(0); b++)
        {
            if(b == invSelected)
            {
                LeanTween.alpha(inv[b].iconHolder.rectTransform, 1f, 0.1f);
            }
            else
            {
                LeanTween.alpha(inv[b].iconHolder.rectTransform, 0.4f, 0.1f);
            }
        }
        ThirdPersonUIHandler.master.UpdateSlotSelectorPos (inv [i].gameObject);
	//	}
	//	else
	//		throw new InventoryFullException ("AlreadySelectedSlot(" + i + ")");
	}

}

class InventoryFullException : UnityException {
	public InventoryFullException(string msg = "") : base(msg) { }
}

