using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uTurret_Target : MonoBehaviour {

	private Transform myTransform;
	public int FactionNumber = 1;
	public bool TargetRegistered = false;

	public int TotalPossibleTargets = 0;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
		// Register Target
		if (!TargetRegistered) {
			uTurrets_TargetManager.GlobalAccess.RegisterTarget (this);	
			TargetRegistered = true;
		}

		// Setup Possible Targetting Points
		possibleTargetingPoints = new List<Transform>();
		Collider[] allCollidersOnGameObject = gameObject.GetComponentsInChildren<Collider>();
		if (allCollidersOnGameObject.Length > 0) {
			for (int i = 0; i < allCollidersOnGameObject.Length; i++) {
				possibleTargetingPoints.Add(allCollidersOnGameObject [i].transform);
			}
		}
		TotalPossibleTargets = possibleTargetingPoints.Count;
	}

	private List<Transform> possibleTargetingPoints;

	public Transform GetMainTargetTransform() {
		return myTransform;
	}

	public Transform GetTargetTransform(Transform seekerTransform) {
//		if (possibleTargetingPoints.Count > 0) {
//			int randomTargetingTransformIndex = Random.Range (0, possibleTargetingPoints.Count);
//			Transform activeTarget = possibleTargetingPoints [randomTargetingTransformIndex];
//			activeTarget = DoCloserTargetCheck (possibleTargetingPoints [randomTargetingTransformIndex], seekerTransform);
//			return activeTarget;
//		}
		// Return Our Transform
		return myTransform;
	}

	private Ray losRay;
	private RaycastHit rayHit;
	public Transform GetActiveTargetTransform(Transform seekerTransform, Transform mainTarget) {
		if (possibleTargetingPoints.Count > 0) {
			int randomTargetingTransformIndex = Random.Range (0, possibleTargetingPoints.Count);
			Transform activeTarget = possibleTargetingPoints [randomTargetingTransformIndex];
//			activeTarget = DoCloserTargetCheck (possibleTargetingPoints [randomTargetingTransformIndex], seekerTransform, mainTarget);
			return activeTarget;
		} else {
			return myTransform;
		}
	}
	private Transform DoCloserTargetCheck(Transform activeTarget, Transform seekerTransform, Transform mainTarget) {
//		Debug.Log ("Doing Target Closer Check...");
		Vector3 targetDirection = activeTarget.position - seekerTransform.position;
		losRay = new Ray (seekerTransform.position, targetDirection);
		if (Physics.Raycast (losRay, out rayHit, 200)) {
			if (rayHit.collider.transform.root.transform == mainTarget) {
				return rayHit.collider.transform;
			} else {
				return myTransform;
			}
		}
		return myTransform;
	}

	void OnDestroy() {
		uTurrets_TargetManager.GlobalAccess.UnregisterTarget (this);
	}
}
