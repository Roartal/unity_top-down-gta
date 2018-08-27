using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HitElement : MonoBehaviour {

	[Header("Universal Variables")]
	public HealthManager manager;
	public Rigidbody rb;
	public float damageMultiplicator = 1.0f;
	public float maxDamage = 10.0f;
	[Header("Collisions Only")]
	public float collisionMultiplicator = 1.0f;
	public float minCollisionMagnitude = 1.0f;
	[Header("Projectiles Only")]
	public float triggerRagdollThreshold = 6.0f;
	
	// Won't work sadly
	void OnCollisionEnter(Collision col)
	{
		if (col.impulse.magnitude >= minCollisionMagnitude && col.collider.tag == "WakeUp") {
		//	manager.Damage (Mathf.Clamp(col.impulse.magnitude * collisionMultiplicator, 0, maxDamage), true);
		}
	}

	public void Wake (Vector3 impulse)
	{
		manager.Ragdoll ();
		if (impulse.magnitude >= minCollisionMagnitude) {
			manager.Damage (Mathf.Clamp (impulse.magnitude * collisionMultiplicator, 0, maxDamage), true);
		}
	}

	public void Damage (float damage)
	{
		if(damage > triggerRagdollThreshold)
			manager.Damage (damage * damageMultiplicator,true);
		else
			manager.Damage (damage * damageMultiplicator,false);
	}
}
