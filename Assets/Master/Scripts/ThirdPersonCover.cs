using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCover : MonoBehaviour {

	public ThirdPersonInputModule tpInput;
	public ThirdPersonAimAtMouse aimControl;
	public LayerMask coverMask;
	private int coverNumber = 0;
	bool useCover;

	// Use this for initialization
	void Update () {
		if (useCover && !aimControl.aiming) {
			tpInput.cover = true;
			return;
		} else{
			tpInput.cover = false;
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Cover") {
			coverNumber++;
			if (coverNumber > 0)
				useCover = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Cover") {
			coverNumber--;
			if (coverNumber <= 0) {
				useCover = false;
			}
		}
	}
}
