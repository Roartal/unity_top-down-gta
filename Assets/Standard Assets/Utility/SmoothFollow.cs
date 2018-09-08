using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{

		// The target we are following
		[SerializeField]
		public Transform target;
		public Rigidbody vel;
        public bool followVel = true;
		// The distance in the x-z plane to the target
		public float dampAmount = 1; 
		public float zoom = 0;
		public float zoomMax = 2f;
		public Transform cam;
        public GameObject shakeGameObject;

		[SerializeField] private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
		[Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
		[SerializeField] private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
		[SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
		[SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
		[SerializeField] private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
		[SerializeField] private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return

		private float m_LookAngle;                    // The rig's y axis rotation.
		private float m_TiltAngle;                    // The pivot's x axis rotation.
		private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
		private Vector3 m_PivotEulers;
		private Quaternion m_PivotTargetRot;
		private Quaternion m_TransformTargetRot;

        public float lastframesvelocity;
        float gforce;
        float eulerZ;
		// Update is called once per frame
		void FixedUpdate()
		{
            cam.localPosition = Vector3.Lerp(cam.localPosition, new Vector3 (cam.localPosition.x, 60 - zoom, -14 + zoom/7),Time.deltaTime * dampAmount);
			// Early out if we don't have a target
			if (!target)
				return;
            if (followVel && vel)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + vel.velocity * 0.3f + new Vector3(shakeGameObject.transform.rotation.x, shakeGameObject.transform.rotation.x, shakeGameObject.transform.rotation.x), Time.deltaTime * dampAmount);
                //G-Force
                gforce = (vel.velocity.magnitude - lastframesvelocity) / (Time.deltaTime * Physics.gravity.magnitude);
                if(Mathf.Abs(gforce)> 3)
                {
                    print(gforce);
                }
                lastframesvelocity = vel.velocity.magnitude;

            }
            else
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * dampAmount);
		}

        void Update()
		{
			zoom += Input.GetAxis ("Mouse ScrollWheel") * 10;
			zoom = Mathf.Clamp (zoom, 0, zoomMax);
			if (Input.GetButton("RotateCam"))
				HandleRotationMovement ();
		}

		private void HandleRotationMovement()
		{
			if(Time.timeScale < float.Epsilon)
				return;

			// Read the user input
			var x = CrossPlatformInputManager.GetAxis("Mouse X");
			var y = CrossPlatformInputManager.GetAxis("Mouse Y");

			// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
			m_LookAngle += x*m_TurnSpeed;

			// Rotate the rig (the root object) around Y axis only:
			m_TransformTargetRot = Quaternion.Euler(m_TiltAngle, m_LookAngle, 0f);

			// on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
			m_TiltAngle -= y*m_TurnSpeed;
			// and make sure the new value is within the tilt range
			m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);

			// Tilt input around X is applied to the pivot (the child of this object)
			m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y , m_PivotEulers.z);

			if (m_TurnSmoothing > 0)
			{
				//transform.localRotation = Quaternion.Slerp(transform.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			}
			else
			{
				//m_Pivot.localRotation = m_PivotTargetRot;
				transform.localRotation = m_TransformTargetRot;
			}
		}
	}
}