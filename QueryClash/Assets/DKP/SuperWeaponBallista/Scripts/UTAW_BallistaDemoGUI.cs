using UnityEngine;
using System.Collections;

public enum GenerationGUIStates {
	Hidden,
	Controls
}

public class UTAW_BallistaDemoGUI : MonoBehaviour {

	public GenerationGUIStates CurrentState = GenerationGUIStates.Controls;

	public UTAW_UltimateBallista BallistaScript;

	public bool GUIHiddenForRendering = false;
	public Transform CameraTransform;
	public float CameraRotationSpeed = 25.0f;
	public float CameraVerticleMoveSpeed = 2.5f;
	public float CameraZoomSpeed = 2.0f;

	private Transform cameraTargetTransform;

	public GUIStyle demoStyle = new GUIStyle();

	// Use this for initialization
	void Start () {
		cameraTargetTransform = gameObject.transform;

		demoStyle.richText = true;
		demoStyle.alignment = TextAnchor.UpperLeft;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetKey (KeyCode.A)) {
			// Rotate Camera Left
			cameraTargetTransform.RotateAround(cameraTargetTransform.position, Vector3.up, CameraRotationSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			// Rotate Camera Left
			cameraTargetTransform.RotateAround(cameraTargetTransform.position, Vector3.up, -CameraRotationSpeed * Time.deltaTime);
		}
		if (CameraTransform != null) {
			if (Input.GetKey (KeyCode.E)) {
				// Zoom Camera In
				Vector3 zoomInDirection = CameraTransform.localPosition - cameraTargetTransform.localPosition;
				CameraTransform.Translate(zoomInDirection * CameraZoomSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.Q)) {
				// Zoom Camera Out
				Vector3 zoomInDirection = cameraTargetTransform.localPosition - CameraTransform.localPosition;
				CameraTransform.Translate(zoomInDirection * CameraZoomSpeed * Time.deltaTime);
			}	
			if (Input.GetKeyUp (KeyCode.R)) {
				// Reset Camera To Original Position
				cameraTargetTransform.rotation = Quaternion.Euler(0, 45, 0);
				CameraTransform.localPosition = new Vector3 (0, 3, 12);
			}
		}
		if (Input.GetKeyUp (KeyCode.F5)) {
			if (CurrentState == GenerationGUIStates.Controls) {
				CurrentState = GenerationGUIStates.Hidden;
			} else if (CurrentState == GenerationGUIStates.Hidden) {
				CurrentState = GenerationGUIStates.Controls;
			}
		}
		if (Input.GetKeyUp (KeyCode.F1)) {
			GUIHiddenForRendering = !GUIHiddenForRendering;
		}
		if (BallistaScript != null) {
			if (Input.GetKeyUp (KeyCode.T)) {
				BallistaScript.ChangeBowConfig ();
			}
		}
	}

	// GUI For Starship Generation DEMO Scene
	void OnGUI () {
		if (!GUIHiddenForRendering) {
			GUI.Label (new Rect (10, 10, 480, 20), "UTAW - Super Weapons - Ballista - Daniel Kole Productions");
			string guidisplayString = "Ballista - Demo - <F5>=Toggle GUI (Controls/Hidden) : " + CurrentState.ToString ();
			GUI.Label (new Rect (20, 30, 580, 20), guidisplayString);

			if (CurrentState == GenerationGUIStates.Controls) {
				GUI.Label (new Rect (10, 80, 340, 20), "Currently Generated Starship Info:");

				GUI.Label (new Rect (10, 120, 340, 20), "Camera Controls:");
				GUI.Label (new Rect (10, 140, 340, 20), "<A><D>: Move Camera Left/Right");
				GUI.Label (new Rect (10, 160, 340, 20), "<Q><E>: Zoom Camera In/Out");
				GUI.Label (new Rect (10, 180, 340, 20), "<R>: Reset Camera To Original Position");
				GUI.Label (new Rect (10, 200, 480, 20), "<T>: Change Ballista Bow Configuration (Open/Closed) - : " + BallistaScript.CurrentBallistaConfig.ToString ());
			} else if (CurrentState == GenerationGUIStates.Hidden) {
			
			}
		}
	}
}
