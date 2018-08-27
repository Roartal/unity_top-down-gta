using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonHealthManager : MonoBehaviour {

	public ThirdPersonRagdoll tpRagdoll;
	public float maxHealth = 10f;
	float health;
	bool isDead = false;
	bool canTakeDamage = true;
	public ParticleSystem splatter;

	void Awake()
	{
		health = maxHealth;
	}

	public void ApplyDamage(float damage)
	{
		if (!isDead && canTakeDamage)
			splatter.Emit (1);
			health -= damage;
			if (health <= 0f)
				Die ();
		ThirdPersonUIHandler.master.UpdateHealth (health, maxHealth);
	}

	public void Die()
	{
		isDead = true;
		health = 0;
		print ("dead");
	}
}
