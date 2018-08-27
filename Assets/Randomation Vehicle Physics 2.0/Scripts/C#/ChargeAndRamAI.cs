using UnityEngine;
using System.Collections;
[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/AI/ChargeAndRam", 0)]

//Class for ramming AI
public class ChargeAndRamAI : MonoBehaviour
{
	Transform tr;
	Rigidbody rb;
	VehicleParent vp;
	VehicleAssist va;
	VehicleDamage vd;
	public Transform target;
	Transform targetPrev;
	Rigidbody targetBody;
	Vector3 targetPoint;
	bool targetVisible;
	bool targetIsWaypoint;
	VehicleWaypoint targetWaypoint;

	public float followDistance;
	bool close;

	[Tooltip("Percentage of maximum speed to drive at")]
	[Range(0, 1)]
	public float speed = 1;
	float initialSpeed;
	float prevSpeed;
    public float targetVelocity = -1;
    float speedLimit = 1;
	float brakeTime;

	[Tooltip("Mask for which objects can block the view of the target")]
	public LayerMask viewBlockMask;
	Vector3 dirToTarget;//Normalized direction to target
	float lookDot;//Dot product of forward direction and dirToTarget
	float steerDot;//Dot product of right direction and dirToTarget

	float stoppedTime;
	float reverseTime;

	[Tooltip("Time limit in seconds which the vehicle is stuck before attempting to reverse")]
	public float stopTimeReverse = 1;

	[Tooltip("Duration in seconds the vehicle will reverse after getting stuck")]
	public float reverseAttemptTime = 1;

	[Tooltip("How many times the vehicle will attempt reversing before resetting, -1 = no reset")]
	public int resetReverseCount = 1;
	int reverseAttempts;

	[Tooltip("Seconds a vehicle will be rolled over before resetting, -1 = no reset")]
	public float rollResetTime = 3;
	float rolledOverTime;

	public Transform rayPos;

	void Start()
	{
		tr = transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
		va = GetComponent<VehicleAssist>();
		vd = GetComponent<VehicleDamage>();
		initialSpeed = speed;
	}

	void FixedUpdate()
	{
            //Set vehicle inputs
		if (vd.damageTaken <= 0) {
			Debug.DrawRay (transform.position, transform.forward);
			RaycastHit rayHit;
			if (Physics.Raycast (rayPos.position, rayPos.forward, out rayHit, 12f)) {
				vp.SetBrake (1);
				vp.SetAccel (0);
			}
			else
				vp.SetAccel (1);
		} else {
			vp.SetBrake (0);
			vp.SetAccel (0);
			//vp.SetEbrake (1);
		}
	}
}