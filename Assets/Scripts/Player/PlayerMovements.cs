using System;
using UnityEngine;

public class PlayerMovements : PortalTraveller
{
	[SerializeField]
	private Rigidbody _rigidbody;

	[SerializeField]
	private Transform _camera;

	[SerializeField]
	private float _moveSpeed = 5;
	[SerializeField]
	private float _sprintMultiplier = 2;
	[SerializeField]
	private float _airMultiplier = 0.1f;
	[SerializeField]
	private float _moveAcceleration = 5;
	[SerializeField]
	private float _jumpHeight = 2;

	private float _verticalRotation;
	private bool _grounded;
	private int _playerMask;
	private Vector3 _gravity = Physics.gravity;

	private const float JUMP_CD = 0.1f;
	private float _currentJumpCd;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		_playerMask = 1 << LayerMask.NameToLayer("Player");
	}

	private void RotateGravity(Quaternion rot)
	{
		_gravity = rot * _gravity;
	}

	private void SetGravity(Vector3 gravity)
	{
		_gravity = gravity;
	}

	private void UpdateGrounded()
	{
		Vector3 nextVel = _rigidbody.velocity + _gravity * Time.deltaTime;
		Vector3 nextPos = _rigidbody.position + nextVel * Time.deltaTime;
		
		_grounded = Physics.CheckSphere(
			nextPos + transform.up * -0.7f,
			0.3f,
			~_playerMask,
			QueryTriggerInteraction.Ignore);
	}

	public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.Teleport(fromPortal, toPortal, pos, rot);
		
		Quaternion velocityRot = Quaternion.FromToRotation(fromPortal.forward, toPortal.forward);
		_rigidbody.velocity = velocityRot * _rigidbody.velocity;
		
		SetGravity(-toPortal.transform.up * Physics.gravity.magnitude);
	}

	// http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
	private static float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude);
	}

	private void FixedUpdate()
	{
		_rigidbody.AddForce(_gravity, ForceMode.Acceleration);
	}

	private void Update()
	{
		_currentJumpCd = Math.Max(_currentJumpCd - Time.deltaTime, 0);

		UpdateGrounded();
		
		// Camera rotation
		Vector2 lookRotation = new Vector2(
			Input.GetAxis("Mouse X"),
			-Input.GetAxis("Mouse Y")
		);

		_verticalRotation -= lookRotation.y;
        
		_verticalRotation = Mathf.Clamp(_verticalRotation, -89f, 89f);
        
		_camera.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
		
		_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.AngleAxis(lookRotation.x, Vector3.up));

		Vector3 a = transform.position + transform.up * 4;
		Debug.DrawLine(a, a + _gravity.normalized, Color.red);
		
		
		// Player move
		
		int forward = 0;
		forward += Convert.ToInt32(Input.GetKey(KeyCode.Z));
		forward -= Convert.ToInt32(Input.GetKey(KeyCode.S));
	
		int right = 0;
		right += Convert.ToInt32(Input.GetKey(KeyCode.D));
		right -= Convert.ToInt32(Input.GetKey(KeyCode.Q));
		
		Vector3 moveDir = new Vector3(right, 0, forward).normalized;
		Vector3 moveVec = moveDir * _moveSpeed;

		Vector3 velocity = transform.InverseTransformDirection(_rigidbody.velocity);
		Vector3 accelerationVec;
		
		if (_grounded)
		{
			if (Input.GetKey(KeyCode.LeftShift))
				moveVec *= _sprintMultiplier;
			
			accelerationVec = moveVec - velocity;
			accelerationVec *= _moveAcceleration;
			accelerationVec *= Time.deltaTime;
			
			if (Input.GetKey(KeyCode.Space) && _currentJumpCd == 0)
			{
				accelerationVec.y = CalculateJumpVerticalSpeed(_jumpHeight) - velocity.y;
				_currentJumpCd = JUMP_CD;
			}
		}
		else
		{
			velocity.y = 0;

			Vector3 moveVecDelta = moveVec * Time.deltaTime;
			Vector3 nextVelocity = velocity + moveVecDelta;

			if (nextVelocity.sqrMagnitude > _moveSpeed * _moveSpeed) // Equivalent to (nextVelocity.magnitude > _moveSpeed), but faster
			{
				nextVelocity = Vector3.ClampMagnitude(nextVelocity, velocity.magnitude);
			}
			
			accelerationVec = nextVelocity - velocity;
			accelerationVec *= _airMultiplier; // Much lower acceleration when in air
		}
		
		_rigidbody.AddForce(transform.TransformDirection(accelerationVec), ForceMode.VelocityChange);
	}
}