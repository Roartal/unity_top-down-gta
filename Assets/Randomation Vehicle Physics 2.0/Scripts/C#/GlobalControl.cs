using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Scene Controllers/Global Control", 0)]

//Global controller class
public class GlobalControl : MonoBehaviour
{
	[Tooltip("Reload the scene with the 'Restart' button in the input manager")]
	public bool quickRestart = true;
	float initialFixedTime;

	[Tooltip("Mask for what the wheels collide with")]
	public LayerMask wheelCastMask;
	public static LayerMask wheelCastMaskStatic;

	[Tooltip("Mask for objects which vehicles check against if they are rolled over")]
	public LayerMask groundMask;
	public static LayerMask groundMaskStatic;

	[Tooltip("Mask for objects that cause damage to vehicles")]
	public LayerMask damageMask;
	public static LayerMask damageMaskStatic;

	public static int ignoreWheelCastLayer;

	[Tooltip("Frictionless physic material")]
	public PhysicMaterial frictionlessMat;
	public static PhysicMaterial frictionlessMatStatic;

	public static Vector3 worldUpDir;//Global up direction, opposite of normalized gravity direction

	[Tooltip("Maximum segments per tire mark")]
	public int tireMarkLength;
	public static int tireMarkLengthStatic;

	[Tooltip("Gap between tire mark segments")]
	public float tireMarkGap;
	public static float tireMarkGapStatic;

	[Tooltip("Tire mark height above ground")]
	public float tireMarkHeight;
	public static float tireMarkHeightStatic;

	[Tooltip("Lifetime of tire marks")]
	public float tireFadeTime;
	public static float tireFadeTimeStatic;

	void Start()
	{
		initialFixedTime = Time.fixedDeltaTime;
		//Set static variables
		wheelCastMaskStatic = wheelCastMask;
		groundMaskStatic = groundMask;
		damageMaskStatic = damageMask;
		ignoreWheelCastLayer = LayerMask.NameToLayer("Ignore Wheel Cast");
		frictionlessMatStatic = frictionlessMat;
		tireMarkLengthStatic = Mathf.Max(tireMarkLength, 2);
		tireMarkGapStatic = tireMarkGap;
		tireMarkHeightStatic = tireMarkHeight;
		tireFadeTimeStatic = tireFadeTime;
	}


    public static IEnumerator Flicker(Renderer renderer, float initialValue, float flickerValue, float flickerSpeed, float flickerDuration)
    {
        if (initialValue == flickerValue)
        {
            yield break;
        }

        float size = renderer.transform.localScale.x;

        WaitForSeconds flickerTime = new WaitForSeconds(flickerSpeed);
        float flickerStop = Time.time + flickerDuration;

        while (Time.time < flickerStop)
        {
            renderer.material.SetFloat("_FlashAmount", flickerValue);
            LeanTween.scale(renderer.gameObject, Vector3.one * size * (1.1f), flickerSpeed);
            yield return flickerTime;
            LeanTween.scale(renderer.gameObject, Vector3.one * size * (0.9f), flickerSpeed);
            renderer.material.SetFloat("_FlashAmount", initialValue);
            yield return flickerTime;
        }

        renderer.material.SetFloat("_FlashAmount", initialValue); ;
    }

    void Update()
	{
		if (quickRestart)
		{
			if (Input.GetButtonDown("Restart"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				Time.timeScale = 1;
				Time.fixedDeltaTime = initialFixedTime;
			}
		}
	}

	void FixedUpdate()
	{
		worldUpDir = Physics.gravity.sqrMagnitude == 0 ? Vector3.up : -Physics.gravity.normalized;
	}
}