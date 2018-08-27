using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public float health = 10.0f;
	float maxHealth;
	bool isDead = false;
	public Rigidbody[] rigidbodies;
	public BoxCollider[] colliders;
	public Rigidbody reference;
	public Animator anim;
	public bool canRagdoll = true;
	public bool deactivateRagdoll = false;
	public LayerMask layers;

	public bool grounded;

	public WaitForSeconds framesToWait = new WaitForSeconds(2.0f);

	void Awake () {
		maxHealth = health;
		Ragdoll (false);
		//canRagdoll = true;
	}

	void FixedUpdate()
	{
		grounded = Physics.Raycast (reference.transform.position, Vector3.down, 0.5f, layers);
	}

	public void Ragdoll (bool activate = true)
	{
		if (activate) {
			foreach (Rigidbody r in rigidbodies) {
				r.isKinematic = false;
				//r.constraints = RigidbodyConstraints.None;
			}
			foreach (BoxCollider c in colliders) {
				c.enabled = false;
			}
			if(!deactivateRagdoll)
				StartCoroutine (DeactivateRagdoll ());
			anim.enabled = false;
		} else {
			foreach (Rigidbody r in rigidbodies) {
				r.isKinematic = true;
				//r.constraints = RigidbodyConstraints.FreezeAll;
				foreach (BoxCollider c in colliders) {
					c.enabled = true;
				}
			}
				
			anim.enabled = true;
		}
	}

	IEnumerator DeactivateRagdoll()
	{
		deactivateRagdoll = true;
		yield return framesToWait;
		while (reference.velocity.magnitude > 2.0f || !grounded) {
			yield return null;
		}
		Ragdoll (false);
		transform.position = reference.transform.position + GetUp();
		deactivateRagdoll = false;

	}

	Vector3 GetUp()
	{
		RaycastHit hit;
		Transform t = reference.transform;
		if (Physics.Raycast (t.position, Vector3.down, out hit, 1f, layers)) {
			return new Vector3(0,hit.transform.position.y,0);
		} else
			return reference.transform.position;
		
	}

	public void Damage(float damage = 1.0f, bool ragdoll = false)
	{
		if (!isDead) {
			health -= damage;
			if (ragdoll) {
				Ragdoll (true);
			}
			if (health <= 0)
				Die ();
		}
	}
	public void Die()
	{
		isDead = true;
		health = 0;
		print (this + " died!");
		this.enabled = false;
	}
}