using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uTurrets_TargetManager : MonoBehaviour {
	public static uTurrets_TargetManager GlobalAccess;
	void Awake () {
		GlobalAccess = this;

		// Setup Target Lists
		ActiveTargetLists = new List<uTurret_Target>[10];
		for (int i = 0; i < 10; i++) {
			ActiveTargetLists [i] = new List<uTurret_Target> ();
		}
		possibleTargetList = new List<uTurret_Target> ();
	}

	// Target Lists
	public List<uTurret_Target>[] ActiveTargetLists;
	private List<uTurret_Target> possibleTargetList;

	// Faction Target Counts
	public int Faction1TargetCount = 0;
	public int Faction2TargetCount = 0;
	public int Faction3TargetCount = 0;
	public int Faction4TargetCount = 0;
	public int Faction5TargetCount = 0;
	public int Faction6TargetCount = 0;
	public int Faction7TargetCount = 0;
	public int Faction8TargetCount = 0;
	public int Faction9TargetCount = 0;
	public int Faction10TargetCount = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Update Faction Target Counts
		UpdateFactionTargetCounts();
	}

	public Transform GetActiveTarget(Transform seekerTransform, float maxDistance, int friendlyFactionNumber) {
		for (int i = 0; i < 10; i++) {
			if (i != friendlyFactionNumber - 1) {
				if (ActiveTargetLists [i].Count > 0) {			
					possibleTargetList.Clear ();
					for (int j = 0; j < ActiveTargetLists [i].Count; j++) {
						float distanceToTarget = Vector3.Distance (seekerTransform.position, ActiveTargetLists [i] [j].transform.position);
						if (distanceToTarget < maxDistance) {
							possibleTargetList.Add (ActiveTargetLists [i] [j]);
						}
					}
					if (possibleTargetList.Count > 0) {
						int randomTargetIndex = Random.Range (0, possibleTargetList.Count);
						return possibleTargetList [randomTargetIndex].GetTargetTransform(seekerTransform);
					} else {
						return null;
					}
				} else {
					return null;
				}
			}
		}
		return null;
	}

	public void RegisterTarget(uTurret_Target targetIn) {
		ActiveTargetLists [targetIn.FactionNumber - 1].Add (targetIn);
	}
	public void UnregisterTarget(uTurret_Target targetIn) {
		ActiveTargetLists [targetIn.FactionNumber - 1].Remove (targetIn);
	}

	private void UpdateFactionTargetCounts() {
		Faction1TargetCount = ActiveTargetLists [0].Count;
		Faction2TargetCount = ActiveTargetLists [1].Count;
		Faction3TargetCount = ActiveTargetLists [2].Count;
		Faction4TargetCount = ActiveTargetLists [3].Count;
		Faction5TargetCount = ActiveTargetLists [4].Count;
		Faction6TargetCount = ActiveTargetLists [5].Count;
		Faction7TargetCount = ActiveTargetLists [6].Count;
		Faction8TargetCount = ActiveTargetLists [7].Count;
		Faction9TargetCount = ActiveTargetLists [8].Count;
		Faction10TargetCount = ActiveTargetLists [9].Count;
	}
}
