using UnityEngine;
using System.Collections;

public class UltimateTurret_KeepOnTarget : MonoBehaviour {

	private Transform myTransform;

	public Transform TargetTransform;
	public float LerpSpeed = 3.5f;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (TargetTransform != null) {
			myTransform.position = Vector3.LerpUnclamped (myTransform.position, TargetTransform.position, LerpSpeed * Time.deltaTime);
		}
	}
}
