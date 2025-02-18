using UnityEngine;
using System.Collections;

/// <summary>
/// How To Use Open/Close Wall Gate Script
/// 
/// Use this code to open/close gate in another script:
/// 
/// Declare a variable like this on a script:
/// public MSFMC_OpenGate GateScript;
/// 
/// Use this line for calling open gate:
/// GateScript.SendMessage("OpenGate", SendMessageOptions.DontRequireReceiver);
/// 
/// Use this line for calling close gate:
/// GateScript.SendMessage("CloseGate", SendMessageOptions.DontRequireReceiver);
/// 
/// </summary>
public class MSFMC_OpenGate : MonoBehaviour {

	public GameObject MainGateGO;
	private Animation GateAnimation;
	public bool GateOpen = false;

	private bool AnimationPlaying = false;
	private float animationTimer = 0.0f;

	/// <summary>
	/// The time between when open/close can be called to allow animation to finish playing.
	/// </summary>
	public float AnimationDelay = 1.0f;

	void Start() {
		if (MainGateGO != null)
			GateAnimation = MainGateGO.GetComponent<Animation>();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (AnimationPlaying) {
			if (animationTimer > 0) {
				animationTimer -= Time.deltaTime;
			}
			else {
				animationTimer = 0;
				AnimationPlaying = false;
			}
		}
	}

	void OpenGate() {
		if (GateAnimation != null) {
			if (!AnimationPlaying) {
				if (!GateOpen) {
					GateAnimation.Play("Open");
					AnimationPlaying = true;
					animationTimer = AnimationDelay;
					GateOpen = true;
				}
			}
		}
	}

	void CloseGate() {
		if (GateAnimation != null) {
			if (!AnimationPlaying) {
				if (GateOpen) {
					GateAnimation.Play("Close");
					AnimationPlaying = true;
					animationTimer = AnimationDelay;
					GateOpen = false;
				}
			}
		}
	}
}
