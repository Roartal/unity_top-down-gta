using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonObjectHealth : MonoBehaviour {

	public Image healthImage;
	public float maxHealth = 10f;
	float health;
	bool isDead = false;
	bool canTakeDamage = true;
	public ParticleSystem splatter;
    public Renderer rend;

	void Awake()
	{
		health = maxHealth;
	}

	public void ApplyDamage(float damage)
	{
		if (!isDead && canTakeDamage)
			splatter.Emit (1);
		health -= damage;
        StartCoroutine(GlobalControl.Flicker(rend, 0f, 0.2f, 0.1f, 0.5f));
		UpdateHealth (health, maxHealth);
		if (health <= 0f)
			Die ();
	}

	public void UpdateHealth(float health, float maxHealth)
	{
		healthImage.fillAmount = health / maxHealth;
	}

	public void Die()
	{
		ApplyDamage (-100);
		//isDead = true;
		health = 10;
		print ("NPC has perished");
	}
}
