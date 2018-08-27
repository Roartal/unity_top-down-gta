using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonInputModule : MonoBehaviour {

	ThirdPersonAimAtMouse tpAim;
	ThirdPersonCharacterMotor tpMotor;
	ThirdPersonHealthManager tpHealth;
	ThirdPersonRagdoll tpRagdoll;

	public string horizontalMovement = "Horizontal";
	public string verticalMovement = "Vertical";
	 
	public string Jump = "Jump";
	public string Crouch = "Crouch";
	public string Sprint = "Sprint";
	public string Walk = "Walk";

	public string Aim = "Aim";
	public string Fire = "Fire";

	public string Interact = "Interact";
	public string Reload = "Reload";
	public string Ragdoll = "Ragdoll";

	private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;
	private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
	private bool sprint;
	private bool crouch;
	public bool cover = false;
	public bool ragdolling = false;

	private void Start()
	{
		// get the transform of the main camera
		if (Camera.main != null)
		{
			m_Cam = Camera.main.transform;
		}
		else
		{
			Debug.LogWarning(
				"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
		}

		// get the third person character ( this should never be null due to require component )
		tpMotor = GetComponent<ThirdPersonCharacterMotor>();
		tpAim = GetComponent<ThirdPersonAimAtMouse> ();
		tpHealth = GetComponent<ThirdPersonHealthManager> (); 
		tpRagdoll = GetComponent<ThirdPersonRagdoll> ();
	}
	
	void Update()
	{
		//
		//THIRD PERSON RAGDOLL SCRIPT
		//
		if (Input.GetButtonDown (Ragdoll)) {
			if (!tpRagdoll.ragdolling) {
				tpAim.ragdolling = true;
				tpRagdoll.EnableRagdoll ();
			} else if (tpRagdoll.spineRB.velocity.magnitude < 0.2f) {
				tpAim.ragdolling = false;
				tpRagdoll.DisableRagdoll ();
			}
		}
		//
		//THIRD PERSON CHARACTER MOTOR SCRIPT
		//
		if (!ragdolling) {
			crouch = Input.GetButton (Crouch);
			if (!m_Jump) {
				m_Jump = Input.GetButtonDown (Jump);
			}
			sprint = Input.GetButton (Sprint);
			//
			//THIRD PERSON AIM SCRIPT
			//
			if (!tpAim.reloading) {
				//AIMING
				if (Input.GetButton (Aim))
					tpAim.aiming = true;
				else
					tpAim.aiming = false;
				//RELOADING
				if (Input.GetButtonDown (Reload))
					tpAim.RequestReload ();
				//ATTACKING
				if (tpAim.aiming) {
					if (Input.GetButtonDown (Fire)) {
						tpAim.ButtonDown ();
					}
					if (Input.GetButton (Fire)) {
						tpAim.timeFirePressed += Time.deltaTime;
						tpAim.ButtonHold ();
					}
					if (Input.GetButtonUp (Fire)) {
						tpAim.ButtonUp ();
					}
				}
			} else
				tpAim.aiming = false;
		}
	}

	void FixedUpdate()
	{
		if (!ragdolling) {
			float h = Input.GetAxis (horizontalMovement);
			float v = -Input.GetAxis (verticalMovement);

			// calculate move direction to pass to character
			if (m_Cam != null) {
				// calculate camera relative direction to move:
				m_CamForward = Vector3.Scale (m_Cam.forward, new Vector3 (1, 0, 1)).normalized;
				m_Move = v * m_CamForward + h * m_Cam.right;
			} else {
				// we use world-relative directions in the case of no main camera
				m_Move = v * Vector3.forward + h * Vector3.right;
			}
			// walk speed multiplier
			if (Input.GetButton (Walk) && !sprint)
				m_Move *= 0.6f;
			if (sprint && m_Move.magnitude < 0.2f)
				sprint = false;

			// pass all parameters to the character control script
			if (cover)
				tpMotor.Move (m_Move, true, m_Jump, sprint);
			else
				tpMotor.Move (m_Move, crouch, m_Jump, sprint);
			m_Jump = false;
		}
	}
}