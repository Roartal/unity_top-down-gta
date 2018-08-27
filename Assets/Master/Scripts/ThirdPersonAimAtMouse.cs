using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonAimAtMouse : MonoBehaviour {
	[Header("Orientation and Rotation")]
	//PUBLIC
	//
	[Tooltip("The part that will point towards the target.")]
	public Transform aimPos;
	public Transform firePos;
	Animator anim;
	[Tooltip("The part that will rotate to face the target.")]
	public Transform aimSpine;
	public Transform referenceSpine;
	public Transform reticle;
	[HideInInspector]
	public bool aiming = false;
	public LayerMask layerMask;
	public bool verticalAiming = true;
	//INVENTORY BASED VARIABLES
	public AudioSource audiosource;
	public AudioClip revolverClick;
	public AudioClip outOfAmmoClick;
	//PRIVATE
	//
	Quaternion targetRot;
	Quaternion spineRotationLastFrame;
	Vector3 lookTarget;
	Vector3 tempSpineLocalEulerAngles;
	SpriteRenderer reticleSprite;
	Rigidbody mainRB;
	ThirdPersonCharacterMotor tpMotor;
	[Header("Shooting")]
	public ParticleSystem impactParticles;

	public ParticleSystem muzzleParticles;
	public ParticleSystem shellParticles;
	public ParticleSystem shellParticlesSmall;
	public ParticleSystem shellParticlesMedium;
	public ParticleSystem shellParticlesHeavy;
	public ParticleSystem shellParticlesShotgun;
	public GameObject linePrefab;

	public Renderer[] meshIDs;

	public bool auto = false;
	public bool semi = true;
	public bool revolver = false;
	public bool reloading = false;

	private float timeBetweenAttacks;
	private float nextAttack;
	private float previousAttack;
	private InventoryItem curItem;
	private float b;
	public float timeFirePressed;
	public bool ragdolling;

	private ParticleSystem.MainModule impactColor;

	public InventoryItem GetCurItem()
	{
		return curItem;
	}

	public void ButtonDown()
	{
		if(semi)
			Attack ();
		if (revolver)
			audiosource.PlayOneShot (revolverClick);
	}
	public void ButtonHold()
	{
		if(auto) //auto
			Attack ();
	}
	public void ButtonUp()
	{
		if (revolver) {
			Attack ();
			timeFirePressed = 0;
		}
	}

	public void RequestReload()
	{
		if (curItem.melee)
			return;
		reloading = true;
		int ammoLeft = 1;
		ammoLeft = bulletIDFinder ();
		if (ammoLeft > 0) {
			int ammoRequired = curItem.ammoPerMag - curItem.ammoInMag;
			if (ammoLeft >= ammoRequired)
				StartCoroutine (Reload (ammoRequired));
			else if (ammoLeft < ammoRequired)
				StartCoroutine (Reload (ammoLeft));
		} else {
			print ("No ammo left!");
			reloading = false;
		}
	}

	void Update()
	{
		anim.SetBool ("Aiming", aiming);
	}

	void Awake()
	{
		impactColor = impactParticles.main;
		mainRB = GetComponent<Rigidbody> ();
		tpMotor = GetComponent<ThirdPersonCharacterMotor> ();
		anim = GetComponent<Animator> ();
		reticleSprite = reticle.GetComponentInChildren<SpriteRenderer> ();
	}

	public void SwitchWeapon(InventoryItem item, bool updateUI = true)
	{
		StopAllCoroutines ();
		reloading = false;
		curItem = item;
		anim.SetFloat ("meshID", curItem.meshID);
		timeBetweenAttacks = 60 / curItem.fireRate; 
		//UPDATE FIREMODE
		semi = false;
		auto = false;
		revolver = false;
		if (curItem.fireMode == 0)
			semi = true;
		else if (curItem.fireMode == 1)
			auto = true;
		else
			revolver = true;
		//VISUALS
		DisplayWeaponMesh ();
		ThirdPersonUIHandler.master.UpdateAmmo (curItem.ammoInMag, bulletIDFinder (), curItem.ammoPerMag, curItem.melee, updateUI);
		switch (curItem.bulletID) {
		case 0:
			shellParticles = shellParticlesSmall; //small
			break;
		case 1:
			shellParticles = shellParticlesMedium; //medium
			break;
		case 2:
			shellParticles = shellParticlesHeavy; //heavy
			break;
		case 3:
			shellParticles = shellParticlesShotgun; //shotgun
			break;
		}
	}

	void OnEnable()
	{
		if(curItem != null && curItem.itemID != -1)
			anim.SetFloat ("meshID", curItem.meshID);
	}

	public IEnumerator Reload(int ammoToReload)
	{
		yield return new WaitForSeconds (curItem.reloadTime);
		switch (curItem.bulletID) {
		case 0:
			ThirdPersonInventory.staticInv.smallAmmoCount -= ammoToReload;
			break;
		case 1:
			ThirdPersonInventory.staticInv.mediumAmmoCount -= ammoToReload;
			break;
		case 2:
			ThirdPersonInventory.staticInv.heavyAmmoCount -= ammoToReload;
			break;
		case 3:
			ThirdPersonInventory.staticInv.shotgunAmmoCount -= ammoToReload;
			break;
		}
		if (!curItem.ejectShells)
			shellParticles.Emit (ammoToReload);
		curItem.ammoInMag += ammoToReload;
		ThirdPersonUIHandler.master.UpdateAmmo (curItem.ammoInMag,bulletIDFinder(),curItem.ammoPerMag);
		anim.SetTrigger ("Reload");
		reloading = false;
	}

	IEnumerator AttackC()
	{
		if (Time.time < nextAttack)
			yield break;
		nextAttack = Time.time + timeBetweenAttacks;
		//
		//RANGED WEAPONRY
		//
		if (!curItem.melee) {
			if (curItem.ammoInMag <= 0) {
				audiosource.PlayOneShot (outOfAmmoClick);
				yield break;
			}
			//ANIM
			anim.SetTrigger ("Fire");
			//VALUES
			curItem.ammoInMag--;
			//UI
			ThirdPersonUIHandler.master.UpdateAmmo (curItem.ammoInMag, bulletIDFinder (), curItem.ammoPerMag);
			//VISUALS
			audiosource.PlayOneShot (curItem.fireSound);
			for (int i = 0; i < curItem.bulletsPerShot; i++)
				GenerateRay ();

			previousAttack = Time.time;
			//RECOIL PHYSICS
			mainRB.AddForce ((-aimPos.forward * calculatedPhysicsRecoil()) + Vector3.up/25*curItem.verticalRecoilMultiplier, ForceMode.Impulse);
			if (curItem.ejectShells)
				shellParticles.Emit (1);
		}
		//
		//MELEE WEAPONRY
		//
		else {
			//VALUES
			previousAttack = Time.time;
			if (curItem.useDurability) {
				curItem.durability--;
				print (curItem.durability);
				curItem.calcDurability = curItem.durability / curItem.maxDurability; //This is for the UI!
				if (curItem.durability <= 0) {
					//DESTROY THE ITEM, IT IS BROKEN!
					audiosource.PlayOneShot (outOfAmmoClick);
					yield break;
				}
			}
			//ANIM
			anim.SetTrigger ("Fire");
			//RECOIL PHYSICS
			mainRB.AddForce ((-aimPos.forward * calculatedPhysicsRecoil()) + Vector3.up/25*curItem.verticalRecoilMultiplier, ForceMode.Impulse);
			audiosource.PlayOneShot (curItem.fireSound);
			yield return new WaitForSeconds (0.1f);
			GenerateMelee ();
		}
	}
	void Attack()
	{
		StartCoroutine (AttackC ());
		return;
		if (Time.time < nextAttack)
			return;
		nextAttack = Time.time + timeBetweenAttacks;
		//
		//RANGED WEAPONRY
		//
		if (!curItem.melee) {
			if (curItem.ammoInMag <= 0) {
				audiosource.PlayOneShot (outOfAmmoClick);
				return;
			}
			//ANIM
			anim.SetTrigger ("Fire");
			//VALUES
			curItem.ammoInMag--;
			//VISUALS
			for (int i = 0; i < curItem.bulletsPerShot; i++)
				GenerateRay ();
			if (curItem.ejectShells)
				shellParticles.Emit (1);
			//UI
			ThirdPersonUIHandler.master.UpdateAmmo (curItem.ammoInMag, bulletIDFinder (), curItem.ammoPerMag);
		}
		//
		//MELEE WEAPONRY
		//
		else {
			//VALUES
			if (curItem.useDurability) {
				curItem.durability--;
				curItem.calcDurability = curItem.durability / curItem.maxDurability; //This is for the UI!
				if (curItem.durability <= 0) {
					//DESTROY THE ITEM, IT IS BROKEN!
					audiosource.PlayOneShot (outOfAmmoClick);
					return;
				}
			}
			//ANIM
			anim.SetTrigger ("Fire");
			GenerateMelee ();
		}
		//RECOIL PHYSICS
		previousAttack = Time.time;
		mainRB.AddForce ((-aimPos.forward * calculatedPhysicsRecoil()) + Vector3.up/25*curItem.verticalRecoilMultiplier, ForceMode.Impulse);
		audiosource.PlayOneShot (curItem.fireSound);
	}

	void GenerateRay()
	{
		GameObject lineObj = Instantiate (linePrefab);
		LineRenderer line = lineObj.GetComponent<LineRenderer> ();
		b = curSpread ();
		//Ray shootRay = new Ray (firePos.position, ((reticle.position - firePos.position) + (new Vector3(Random.Range(-b,b),0,Random.Range(-b,b)))));
		//Debug.DrawRay(firePos.position,((reticle.position - firePos.position).normalized * 500f),Color.yellow,1.0f);
		//print (b);
		Vector3 u = (reticle.position - firePos.position).normalized;
		if (ragdolling)
			u = firePos.forward;
		Ray shootRay = new Ray (firePos.position, (u + (new Vector3(Random.Range(-b,b),0,Random.Range(-b,b)))));
		RaycastHit hitRanged;
		if (Physics.Raycast (shootRay, out hitRanged, 100f, layerMask)) {
            impactColor.startColor = Color.white;
            if(hitRanged.collider.GetComponent<Renderer>())
            {
                impactColor.startColor = hitRanged.collider.GetComponent<Renderer>().material.GetColor("_Color");
            }
			if (hitRanged.collider.attachedRigidbody)
				hitRanged.collider.attachedRigidbody.AddForce (shootRay.direction * 10f);
			if (hitRanged.collider.GetComponent<ThirdPersonHealthHit> ()) {
				hitRanged.collider.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (curItem.damage);
				impactColor.startColor = Color.red;
			}
			if (hitRanged.collider.GetComponent<Hitable> ()) {
				impactColor.startColor = hitRanged.collider.GetComponent<Hitable>().GetColor();
			}
			line.SetPosition (0, firePos.position);
			line.SetPosition (1, hitRanged.point);
			impactParticles.transform.position = hitRanged.point;
			impactParticles.Emit (Random.Range (5, 10));
			muzzleParticles.Emit (5);
            print("HIT " + hitRanged.collider.gameObject.name + " / " + impactColor.startColor.ToString());
		} else {
			line.SetPosition (0, firePos.position);
			line.SetPosition (1, firePos.forward * 100);
			print ("NO HIT");
			muzzleParticles.Emit (5);
		}
	}

	public void GenerateMelee()
	{
		/* OLD METHOD WITH SPHERE STUFF
		Collider[] objectColliders = Physics.OverlapSphere(this.transform.position, curItem.meleeDistance, layerMask);
		for (int index = 0; index <= Mathf.Clamp(objectColliders.Length - 1,0,3); index++) {
			GameObject colliderObject = objectColliders [index].gameObject;
			if (Vector3.Distance(colliderObject.transform.position,transform.position) > .8f) {
				if (colliderObject.GetComponent<Rigidbody> ())
					colliderObject.GetComponent<Rigidbody> ().AddForce (-firePos.forward * 10f);
				if (colliderObject.GetComponent<ThirdPersonHealthHit> ())
					colliderObject.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (curItem.damage);
			}
		}*/
		RaycastHit hitsLeft;
		RaycastHit hitsCenter;
		RaycastHit hitsRight;
		if (Physics.Raycast (aimPos.position, aimPos.forward, out hitsCenter, curItem.meleeDistance, layerMask)) {
			if (hitsCenter.collider.attachedRigidbody)
				hitsCenter.collider.attachedRigidbody.AddForce ((hitsCenter.collider.transform.position - aimSpine.position) * 10f);
			if (hitsCenter.collider.GetComponent<ThirdPersonHealthHit> ())
				hitsCenter.collider.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (curItem.damage);
		}
		if (Physics.Raycast (aimPos.position, aimPos.forward + aimPos.right, out hitsRight, curItem.meleeDistance, layerMask)) {
			if (hitsRight.collider.attachedRigidbody)
				hitsRight.collider.attachedRigidbody.AddForce ((hitsRight.collider.transform.position - aimSpine.position) * 10f);
			if (hitsRight.collider.GetComponent<ThirdPersonHealthHit> ())
				hitsRight.collider.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (curItem.damage);
		}
		if (Physics.Raycast (aimPos.position, aimPos.forward - aimPos.right, out hitsLeft, curItem.meleeDistance, layerMask)) {
			if (hitsLeft.collider.attachedRigidbody)
				hitsLeft.collider.attachedRigidbody.AddForce ((hitsLeft.collider.transform.position - aimSpine.position) * 10f);
			if (hitsLeft.collider.GetComponent<ThirdPersonHealthHit> ())
				hitsLeft.collider.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (curItem.damage);
		}
	}
		
	public float curSpread()
	{
		float s = (curItem.spread * 0.333f)/((Time.time - previousAttack)*2);
		float x = Mathf.Round ((s*s/2) * 100)/100 + curItem.spreadConstant / 6;
		return x;
	}

	public int bulletIDFinder()
	{
		int i = 0;
		switch (curItem.bulletID) {
		case -1:
			i = 0;
			break;
		case 0:
			i = ThirdPersonInventory.staticInv.smallAmmoCount;
			break;
		case 1:
			i = ThirdPersonInventory.staticInv.mediumAmmoCount;
			break;
		case 2:
			i = ThirdPersonInventory.staticInv.heavyAmmoCount;
			break;
		case 3:
			i = ThirdPersonInventory.staticInv.shotgunAmmoCount;
			break;
		}
		return i;
	}

	public float calculatedPhysicsRecoil()
	{
		float s = 0;
		if(tpMotor.m_IsGrounded)
			s =  (curItem.damage * curItem.bulletsPerShot/2)/12 * curItem.horizontalRecoilMultiplier;
		else
			s =  (curItem.damage * curItem.bulletsPerShot/2)/24 * curItem.horizontalRecoilMultiplier;
			
		return s;
	}

	void DisplayWeaponMesh()
	{
		if (curItem.meshID != -1) {
			for (int i = 0; i < this.meshIDs.Length; i++) {
					meshIDs [i].enabled = false;
			}
			meshIDs [curItem.meshID].enabled = true;
		}
	}

	void LateUpdate()
	{
		/*if (aiming) {
			float z = (curItem.spread) / (timeBetweenAttacks * 2);
			float u = z * z / 2 + curItem.spreadConstant;
			Debug.DrawRay (firePos.position, ((reticle.position - firePos.position) + (new Vector3 (-u, 0, -u))).normalized * 200, Color.cyan);
			Debug.DrawRay (firePos.position, ((reticle.position - firePos.position) + (new Vector3 (+u, 0, +u))).normalized * 200, Color.cyan);
		}*/
		//ROTATION
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit floorHit;
		if (verticalAiming) {
			if (Physics.Raycast (camRay, out floorHit, 500f, -1, QueryTriggerInteraction.Ignore)) {
				if (floorHit.collider.tag == "MainGround")
					reticle.position = floorHit.point + Vector3.up * 0.5f;
				else
					reticle.position = floorHit.point;
				if (reticle && aiming) {
					aimSpine.rotation = Quaternion.FromToRotation (aimPos.forward, floorHit.point - aimPos.position) * aimSpine.rotation;

					tempSpineLocalEulerAngles = aimSpine.localEulerAngles;
					tempSpineLocalEulerAngles = new Vector3 (tempSpineLocalEulerAngles.x, tempSpineLocalEulerAngles.y, 0);
					aimSpine.localEulerAngles = tempSpineLocalEulerAngles;

					targetRot = aimSpine.rotation;

					aimSpine.rotation = Quaternion.Slerp (spineRotationLastFrame, targetRot, Time.deltaTime * 24);
					spineRotationLastFrame = aimSpine.rotation;
					reticleSprite.enabled = true;
				} else {
					aimSpine.rotation = Quaternion.Slerp (spineRotationLastFrame, referenceSpine.rotation, Time.deltaTime * 12);
					spineRotationLastFrame = aimSpine.rotation;
					reticleSprite.enabled = false;
				}
			}
		}
		else
		{
			if (reticle && aiming) {
				reticleSprite.enabled = true;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Plane plane = new Plane (Vector3.up, Vector3.zero);
				float distance;
				if (plane.Raycast (ray, out distance)) {
					Vector3 target = ray.GetPoint (distance);
					Vector3 direction = target - transform.position;
					reticle.position = target;
					float rotation = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
					aimSpine.rotation = Quaternion.Euler (0, rotation, 0);
					spineRotationLastFrame = aimSpine.rotation;
				}
			} else {
				aimSpine.rotation = Quaternion.Slerp (spineRotationLastFrame, referenceSpine.rotation, Time.deltaTime * 12);
				spineRotationLastFrame = aimSpine.rotation;
				reticleSprite.enabled = false;
			}
		}
	}
}