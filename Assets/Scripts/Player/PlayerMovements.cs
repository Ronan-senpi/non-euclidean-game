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

	private Vector2 _camRot;
	private bool _grounded;
	private int _playerMask;

	private const float JUMP_CD = 0.1f;
	private float _currentJumpCd;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		_playerMask = 1 << LayerMask.NameToLayer("Player");
	}

	private void UpdateGrounded()
	{
		Vector3 nextVel = _rigidbody.velocity + Physics.gravity * Time.deltaTime;
		Vector3 nextPos = _rigidbody.position + nextVel * Time.deltaTime;
		
		_grounded = Physics.CheckSphere(
			nextPos + transform.up * -0.7f,
			0.3f,
			~_playerMask);
	}

	// http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
	private static float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude);
	}

	public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.Teleport(fromPortal, toPortal, pos, rot);
		
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

		_camRot.x += lookRotation.x;
		_camRot.y -= lookRotation.y;
		
		_camRot.y = Mathf.Clamp(_camRot.y, -89f, 89f);
		
		_camera.localRotation = Quaternion.Euler(_camRot.y, 0, 0);

		Vector3 upDir = -Physics.gravity.normalized;
		Vector3 forwardDir = Quaternion.FromToRotation(Vector3.up, upDir) * Quaternion.AngleAxis(_camRot.x, Vector3.up) * Vector3.forward;
		_rigidbody.MoveRotation(Quaternion.LookRotation(forwardDir, upDir));
		
		
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