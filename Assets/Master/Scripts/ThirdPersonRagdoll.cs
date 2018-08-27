using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonRagdoll : MonoBehaviour {

	public Animator anim;
	public Rigidbody mainRB;
	public CapsuleCollider mainCol;
	public GameObject root;
	public ThirdPersonAimAtMouse tpAimScript;
	public ThirdPersonCharacterMotor tpMotor;
	public Transform spine;
	public Rigidbody spineRB;
	[HideInInspector]
	public bool ragdolling = false;
	public Rigidbody[] RBs;
	public UnityStandardAssets.Utility.SmoothFollow cam;
	public ThirdPersonHealthManager tpHealth;

	public ThirdPersonInputModule tpInput;

	public AudioClip[] hits;
	public AudioSource audiosource;

	public void OnCollisionEnter(Collision col)
	{
		if (col.rigidbody) {
			if (col.rigidbody.velocity.magnitude > 5f) {
				EnableRagdoll ();
				tpHealth.ApplyDamage (Mathf.Clamp (col.rigidbody.velocity.magnitude, 0, 9));
				audiosource.PlayOneShot(hits[Random.Range(0,hits.Length)]);
			}
		}

		if (!tpMotor.m_IsGrounded && col.impulse.magnitude > 0.1f) {
			EnableRagdoll ();
			tpHealth.ApplyDamage (Mathf.Clamp(col.impulse.magnitude,0,9));
			audiosource.PlayOneShot(hits[Random.Range(0,hits.Length)]);
		}
	}
	void Update () {
		if (ragdolling)
			transform.position = spine.transform.position;
	}
	
	public void EnableRagdoll(float multi = 1f)
	{
		spineRB.velocity = mainRB.velocity * multi;
		tpInput.ragdolling = true;
		tpAimScript.enabled = false;
		ragdolling = true;
		//mainRB.constraints = RigidbodyConstraints.FreezeAll;
		mainRB.isKinematic = true;
		mainCol.enabled = false;
		anim.enabled = false;
		root.transform.SetParent (null);
		cam.vel = spineRB;
		foreach (Rigidbody r in RBs) {
			r.velocity = spineRB.velocity;
		}
	}

	public void DisableRagdoll()
	{
		tpMotor.Move (Vector3.zero, false, false);
		tpInput.ragdolling = false;
		tpAimScript.enabled = true;
		ragdolling = false;
		anim.enabled = true;
		//mainRB.constraints = RigidbodyConstraints.FreezeRotation;
		mainRB.isKinematic = false;
		mainCol.enabled = true;
		cam.vel = mainRB;
		root.transform.SetParent (transform);
	}
}
