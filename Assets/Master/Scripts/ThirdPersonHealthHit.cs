using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonHealthHit : MonoBehaviour {

	public ThirdPersonHealthManager tpHealth;
	public ThirdPersonObjectHealth tpObject;
	public float damageMultiplier = 1;

	public void ApplyDamage(float damage)
	{
		float newDamage = damage * damageMultiplier;
		if(tpHealth)
			tpHealth.ApplyDamage (newDamage);
		if (tpObject)
			tpObject.ApplyDamage (newDamage);
	}

	public void ForceRagdoll()
	{
		if (tpHealth)
			tpHealth.tpRagdoll.EnableRagdoll ();
	}
}
