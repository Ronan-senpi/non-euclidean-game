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
	
	[SerializeField]
	[Range(0, 1)]
	private float _feetFriction = 0.3f;

	[SerializeField]
	private MeshCollider _feetCollider;

	private float _verticalRotation;
	private bool _grounded;
	
	private Vector3 GravityDir { get; set; } = Physics.gravity.normalized;
	private float GravityMag { get; set; } = Physics.gravity.magnitude;
	private Vector3 Gravity => GravityDir * (GravityMag * transform.lossyScale.x);

	private const float JUMP_CD = 0.1f;
	private float _currentJumpCd;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;

		_feetCollider.material = new PhysicMaterial
		{
			dynamicFriction = _feetFriction,
			staticFriction = _feetFriction,
			bounceCombine = PhysicMaterialCombine.Minimum,
			frictionCombine = PhysicMaterialCombine.Minimum
		};
	}

	public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.Teleport(fromPortal, toPortal, pos, rot);
		
		Quaternion velocityRot = Quaternion.FromToRotation(fromPortal.forward, toPortal.forward);
		_rigidbody.velocity = velocityRot * _rigidbody.velocity;

		Vector3 localGravityDir = fromPortal.InverseTransformDirection(GravityDir);
		GravityDir = toPortal.TransformDirection(localGravityDir);
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
		_rigidbody.AddForce(Gravity, ForceMode.Acceleration);
		_grounded = false;
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.GetContact(0).thisCollider == _feetCollider)
		{
			_grounded = true;
		}
	}

	private void Update()
	{
		_feetCollider.material.dynamicFriction = _feetFriction * transform.lossyScale.x;
		_feetCollider.material.staticFriction = _feetFriction * transform.lossyScale.x;
		
		_currentJumpCd = Math.Max(_currentJumpCd - Time.deltaTime, 0);
		
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
		Debug.DrawLine(a, a + GravityDir, Color.red);
		
		
		// Player move
		
		int forward = 0;
		forward += Convert.ToInt32(Input.GetKey(KeyCode.Z));
		forward -= Convert.ToInt32(Input.GetKey(KeyCode.S));
	
		int right = 0;
		right += Convert.ToInt32(Input.GetKey(KeyCode.D));
		right -= Convert.ToInt32(Input.GetKey(KeyCode.Q));
		
		Vector3 moveDir = new Vector3(right, 0, forward).normalized;
		Vector3 moveVec = moveDir * _moveSpeed;

		Vector3 velocity = transform.InverseTransformVector(_rigidbody.velocity);
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
		
		_rigidbody.AddForce(transform.TransformVector(accelerationVec), ForceMode.VelocityChange);
	}
}