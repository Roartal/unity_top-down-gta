using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonExplosiveObjectHealth : MonoBehaviour {

	AudioSource aud;
	public AudioClip explodeAud;
	public AudioClip fuzeAud;
	public AudioClip leakAud;
	public float maxHealth = 10f;
	float health;
	bool isDead = false;
	public bool isLeaking = false;
	public bool isBurning = false;
	public ParticleSystem splatter;
	public ParticleSystem leak;
	public ParticleSystem.EmissionModule leakEmission;

	public ParticleSystem explosion;

	public ParticleSystem fuze;
	public ParticleSystem.EmissionModule fuzeEmission;

	void Awake()
	{
		aud = GetComponent<AudioSource> ();
		fuzeEmission = fuze.emission;
		fuzeEmission.enabled = false;
		leakEmission = leak.emission;
		leakEmission.enabled = false;
		health = maxHealth;
	}

	public void ApplyDamage(float damage, bool firearm = true)
	{
		//FUZE
		if (firearm && !isBurning && damage > 3f) {
			isBurning = true;
			StartCoroutine(StartFuze ());
		}

		//HIT
		if (!isDead)
			splatter.Emit (1);
		
		health -= damage;

		//EXPLOSION
		if (health <= 0f && !isDead) {
			Die ();
			return;
		}
		if (!isLeaking && !isBurning) {
			isLeaking = true;
			leakEmission.enabled = true;
		}
	}

	IEnumerator StartFuze()
	{
		fuzeEmission.enabled = true;
		yield return new WaitForSeconds (5.0f);
		Die ();
	}

	public void Die()
	{
		aud.PlayOneShot (explodeAud);
		explosion.Emit (1);
		fuzeEmission.enabled = false;
		leakEmission.enabled = false;
		isDead = true;
		float r = 5;
		var cols = Physics.OverlapSphere(transform.position, r);
		var rigidbodies = new List<Rigidbody>();
		foreach (var col in cols)
		{
			if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
			{
				rigidbodies.Add(col.attachedRigidbody);
			}
		}
		foreach (var rb in rigidbodies)
		{
			if (rb.GetComponent<ThirdPersonHealthHit> ()) {
				rb.GetComponent<ThirdPersonHealthHit> ().ApplyDamage (Mathf.Clamp (3 / Vector3.Distance (rb.transform.position, transform.position), 0f, 10f));
				rb.GetComponent<ThirdPersonHealthHit> ().ForceRagdoll ();
			}
			rb.AddExplosionForce(0.333f, transform.position, r, 1, ForceMode.Impulse);
		}
		print ("EXPLOSION!");
	}
}
