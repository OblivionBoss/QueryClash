using UnityEngine;
using System.Collections;

public class uTurret_LookAtTarget : MonoBehaviour {

	private Transform myTransform;
	public Transform LookAtTransform;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (LookAtTransform != null) {
			myTransform.LookAt (LookAtTransform.position);
		}
	}
}
