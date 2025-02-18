using UnityEngine;
using System.Collections;

public enum MouseLocationStates {
	Left,
	Centered,
	Right
}

public class UTAWTargetingCamera : MonoBehaviour {

	private Transform myTransform;

	public Transform MainCameraTransform;
	public Transform TargetPointTransform;

	// Current Targeting Mode
	public UTAW_TurretTargetingModes CurrentTargetingMode = UTAW_TurretTargetingModes.AutomatedTargeting;
	private bool fireTurrets = false;

	// Forward Point Sensor
	public Transform ForwardPointSensor;
	private float forwardSensingTimer = 0;
	private float forwardSensingTimerFreq = 0.25f;

	// Mouse Rotation
	public float MouseRotationSensitivity = 0.05f;
	public MouseLocationStates MouseLocState = MouseLocationStates.Centered;
	private MouseLocationStates lastLocState = MouseLocationStates.Centered;
	public float MouseDistFromCenter = 0;
	private Vector2 currentMousePos;
	public float RotationSpeed = 0;

	public UTAW_TurretController[] ActiveTurrets;

	// Use this for initialization
	void Awake () {
		myTransform = gameObject.transform;

		// Hijack Turret Controls
		if (ActiveTurrets.Length > 0) {
			UpdateActiveTurrets ();
		}

		// Unlink Manual Targeting Point
		if (TargetPointTransform != null) {
			TargetPointTransform.parent = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		// Update Manual Targeting Point Transform Location
		if (CurrentTargetingMode == UTAW_TurretTargetingModes.ManuallyControlled) {
			UpdateManualTargetingPoint ();
		}

		// Update Mouse Rotation
		UpdateMouseRotation();

		// Cycle Targeting Modes
		if (Input.GetKeyUp (KeyCode.Space)) {
			if (CurrentTargetingMode == UTAW_TurretTargetingModes.AutomatedTargeting)
				CurrentTargetingMode = UTAW_TurretTargetingModes.ManuallyControlled;
			else if (CurrentTargetingMode == UTAW_TurretTargetingModes.ManuallyControlled)
				CurrentTargetingMode = UTAW_TurretTargetingModes.AutomatedTargeting;
		}

		// Fire Turrets
		if (ActiveTurrets.Length > 0) {
			if (Input.GetMouseButton (0)) {
				fireTurrets = true;
			} else {
				fireTurrets = false;
			}
		}

		// Update Turrets
		if (ActiveTurrets.Length > 0) {
			UpdateActiveTurrets ();
		}
	}

	private void UpdateActiveTurrets() {
		// Update Manually Controlled Turrets
		for (int i = 0; i < ActiveTurrets.Length; i++) {
			if (ActiveTurrets[i] != null) {
				// Always Check and Update Targeting Mode of Controlled Turrets
				if (ActiveTurrets [i].CurrentTargetMode != CurrentTargetingMode)
					ActiveTurrets [i].CurrentTargetMode = CurrentTargetingMode;
				if (ActiveTurrets [i].ManualTargetTransform != TargetPointTransform)
					ActiveTurrets [i].ManualTargetTransform = TargetPointTransform;
				if (fireTurrets) {
					ActiveTurrets [i].TryManualFiringTurret ();
				}
			}
		}
	}

	// Manual Targeting Variables and Functions
	private Ray targetingRay;
	private RaycastHit targetingHit;
	private void UpdateManualTargetingPoint (){
		targetingRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast(targetingRay, out targetingHit, 200)) {
			float distanceToTargetingPoint = Vector3.Distance (myTransform.position, targetingHit.point);
			if (distanceToTargetingPoint > 10) {
				TargetPointTransform.position = targetingHit.point;
			}
		}
	}

	private void UpdateMouseRotation() {
		currentMousePos = Input.mousePosition;
		bool mouseInScreenArea = true;
		Rect screenRect = new Rect (0, 0, Screen.width, Screen.height);
		if (!screenRect.Contains (Input.mousePosition)) {
			mouseInScreenArea = false;
		}

		float screenWidthMidpoint = Screen.width / 2;
		float percentWindowSize = Screen.width * 0.25f;
		if (mouseInScreenArea) {
			if (currentMousePos.x > screenWidthMidpoint + percentWindowSize) {
				MouseLocState = MouseLocationStates.Right;
				lastLocState = MouseLocationStates.Right;
				MouseDistFromCenter = currentMousePos.x - percentWindowSize - (Screen.width / 2);
				RotationSpeed = MouseDistFromCenter * MouseRotationSensitivity * Time.deltaTime;
				if (RotationSpeed > 0) {
					// Rotate View
					myTransform.RotateAround (myTransform.position, myTransform.up, RotationSpeed);
				}
			} else if (currentMousePos.x < screenWidthMidpoint - percentWindowSize) {
				MouseLocState = MouseLocationStates.Left;
				lastLocState = MouseLocationStates.Left;
				MouseDistFromCenter = (Screen.width / 2) - currentMousePos.x - percentWindowSize;
				RotationSpeed = MouseDistFromCenter * MouseRotationSensitivity * Time.deltaTime;
				if (RotationSpeed > 0) {
					// Rotate View
					myTransform.RotateAround (myTransform.position, myTransform.up, -RotationSpeed);
				}
			} else {
				MouseLocState = MouseLocationStates.Centered;
				RotationSpeed = Mathf.Lerp (RotationSpeed, 0, Time.deltaTime / 2);
				if (lastLocState == MouseLocationStates.Right) {
					// Rotate View
					myTransform.RotateAround (myTransform.position, myTransform.up, RotationSpeed);
				} else {
					// Rotate View
					myTransform.RotateAround (myTransform.position, myTransform.up, -RotationSpeed);
				}
			}				 
		} else {
			// Recenter Mouse Rotation
			MouseLocState = MouseLocationStates.Centered;
			RotationSpeed = Mathf.Lerp (RotationSpeed, 0, Time.deltaTime);
			if (lastLocState == MouseLocationStates.Right) {
				// Rotate View
				myTransform.RotateAround (myTransform.position, myTransform.up, RotationSpeed);
			} else {
				// Rotate View
				myTransform.RotateAround (myTransform.position, myTransform.up, -RotationSpeed);
			}
		}
	}
}
