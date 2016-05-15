using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class CharacterMachine : SuperStateMachine {
	public Transform AnimatedMesh;

	public Transform Head;

	public float WalkSpeed = 4.0f;
	public float WalkAcceleration = 30.0f;
	public float JumpAcceleration = 5.0f;
	public float JumpHeight = 3.0f;
	public float Gravity = 25.0f;
	public float GroundFriction = 10.0f;

	// Add more states by comma separating them
	enum PlayerStates {
		Idle,
		Walk,
		Jump,
		Fall
	}

	private SuperCharacterController controller;
	private PlayerInputController input;
	private Vector3 moveDirection;

	public Vector3 lookDirection { get; private set; }

	private MouseLook mouseLook;

	void Start() {
		input = gameObject.GetComponent<PlayerInputController>();
		controller = gameObject.GetComponent<SuperCharacterController>();
		lookDirection = transform.forward;
		currentState = PlayerStates.Idle;
		mouseLook = gameObject.GetComponent<MouseLook>();//new MouseLook();
		//mouseLook.XSensitivity = 1;
		//mouseLook.YSensitivity = 1;
		mouseLook.Init(transform, Head.transform);
	}

	protected override void EarlyGlobalSuperUpdate() {
		// Rotate out facing direction horizontally based on mouse input
		lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x, controller.up) * lookDirection;
		// Put any code in here you want to run BEFORE the state's update function.
		// This is run regardless of what state you're in

		//currentHeadRotation = Mathf.Clamp(currentHeadRotation + input.Current.MouseInput.y * rotationSpeed, minHeadRotation, maxHeadRotation);
		//headDirection = Quaternion.AngleAxis(input.Current.MouseInput.y, transform.forward) * headDirection;
	}

	protected override void LateGlobalSuperUpdate() {
		// Put any code in here you want to run AFTER the state's update function.
		// This is run regardless of what state you're in

		// Move the player by our velocity every frame
		transform.position += moveDirection * controller.deltaTime;

		// Rotate our mesh to face where we are "looking"
		AnimatedMesh.rotation = Quaternion.LookRotation(lookDirection, controller.up);
		//Head.rotation = Quaternion.LookRotation(headDirection, transform.forward);
		mouseLook.LookRotation(transform, Head.transform);
	}

	private bool AcquiringGround() {
		return controller.currentGround.IsGrounded(false, 0.01f);
	}

	private bool MaintainingGround() {
		return controller.currentGround.IsGrounded(true, 0.5f);
	}

	public void RotateGravity(Vector3 up) {
		lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
	}

	/// <summary>
	/// Constructs a vector representing our movement local to our lookDirection, which is
	/// controlled by the camera
	/// </summary>
	private Vector3 LocalMovement() {
		Vector3 right = Vector3.Cross(controller.up, lookDirection);

		Vector3 local = Vector3.zero;

		if (input.Current.MoveInput.x != 0) {
			local += right * input.Current.MoveInput.x;
		}

		if (input.Current.MoveInput.z != 0) {
			local += lookDirection * input.Current.MoveInput.z;
		}

		return local.normalized;
	}

	// Calculate the initial velocity of a jump based off gravity and desired maximum height attained
	private float CalculateJumpSpeed(float jumpHeight, float gravity) {
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	// Below are the three state functions. Each one is called based on the name of the state,
	// so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
	// Jump_SuperUpdate()
	void Idle_EnterState() {
		controller.EnableSlopeLimit();
		controller.EnableClamping();
	}

	void Idle_SuperUpdate() {
		// Run every frame we are in the idle state

		if (input.Current.JumpInput) {
			currentState = PlayerStates.Jump;
			return;
		}

		if (!MaintainingGround()) {
			currentState = PlayerStates.Fall;
			return;
		}

		if (input.Current.MoveInput != Vector3.zero) {
			currentState = PlayerStates.Walk;
			return;
		}

		// Apply friction to slow us to a halt
		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * controller.deltaTime);
	}

	void Idle_ExitState() {
		// Run once when we exit the idle state
	}

	void Walk_SuperUpdate() {
		if (input.Current.JumpInput) {
			currentState = PlayerStates.Jump;
			return;
		}

		if (!MaintainingGround()) {
			currentState = PlayerStates.Fall;
			return;
		}

		if (input.Current.MoveInput != Vector3.zero) {
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * WalkSpeed, WalkAcceleration * controller.deltaTime);
		} else {
			currentState = PlayerStates.Idle;
			return;
		}
	}

	void Jump_EnterState() {
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
	}

	void Jump_SuperUpdate() {
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround()) {
			moveDirection = planarMoveDirection;
			currentState = PlayerStates.Idle;
			return;
		}

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;

		moveDirection = planarMoveDirection + verticalMoveDirection;
	}

	void Fall_EnterState() {
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		// moveDirection = trueVelocity;
	}

	void Fall_SuperUpdate() {
		if (AcquiringGround()) {
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			currentState = PlayerStates.Idle;
			return;
		}

		moveDirection -= controller.up * Gravity * controller.deltaTime;
	}
}
