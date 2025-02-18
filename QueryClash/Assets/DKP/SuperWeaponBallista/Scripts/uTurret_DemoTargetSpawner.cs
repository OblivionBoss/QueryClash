using UnityEngine;
using System.Collections;

public class uTurret_DemoTargetSpawner : MonoBehaviour {

	private Transform myTransform;
	public GameObject TargetToSpawnPrefab;
	private GameObject spawnedTarget;
	public Transform FacingTransfrom;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnedTarget == null) {
			// Face Starting Transform
			if (FacingTransfrom != null)
				myTransform.LookAt(FacingTransfrom);

			// Spawn A New Target
			Vector3 piecePosition = myTransform.position;
			Vector3 pieceDirectionEulers = new Vector3 (0, 0, 0);
			Quaternion pieceRotation = myTransform.rotation; // Quaternion.Euler (pieceDirectionEulers);

//			int randomTurretIndex = Random.Range (0, PossibleTopTurrets.Length);
//			GameObject prefabToUse = PossibleTopTurrets [randomTurretIndex];
//			string platformNameString = prefabToUse.name;

			GameObject newTargetSpawnedGO = GameObject.Instantiate (TargetToSpawnPrefab, piecePosition, pieceRotation) as GameObject;
			newTargetSpawnedGO.name = TargetToSpawnPrefab.name;

			spawnedTarget = newTargetSpawnedGO;
		}
	}
}
