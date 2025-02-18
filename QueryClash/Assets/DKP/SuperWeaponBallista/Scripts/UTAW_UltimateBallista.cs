using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BallistaStateTypes {
	Inactive,
	Loading,
	Loaded,
	Fired,
	ReloadPause
}

public enum BallistaBowTypes {
	Open,
	Closed
}

public class UTAW_UltimateBallista : MonoBehaviour {

	private Transform myTransform;

	public bool TestFireBallista = false;
	public BallistaStateTypes CurrentState = BallistaStateTypes.Inactive;
	public float ReloadPauseTime = 1.0f;
	private float reloadPauseTimer = 0;

	public bool BallistaChangingConfig = false;
	public BallistaBowTypes CurrentBallistaConfig = BallistaBowTypes.Closed;
	public void ChangeBowConfig() {
		if (!BallistaChangingConfig) {
			if (CurrentBallistaConfig == BallistaBowTypes.Closed) {
				CurrentBallistaConfig = BallistaBowTypes.Open;
				BallistaChangingConfig = true;
			} else {
				CurrentBallistaConfig = BallistaBowTypes.Closed;
				BallistaChangingConfig = true;
			}
		}
	}

	// String Prefabs
	public GameObject BallistaStringPrefab;

	// String Transforms
	public Transform TopStringConnectTransform;
	public Transform[] TopStringPoints;
	public Transform BottomStringConnectTransform;
	public Transform[] BottomStringPoints;
	// Ballista Strings
	private UTAW_BallistaString[] topBallistaStrings;
	private UTAW_BallistaString[] bottomBallistaStrings;

	public Transform BarrelTransform;

	private bool hasPlayedChargingSFX = false;
	private List<AudioSource> ballistaChargeSFXList;
	public AudioClip BallistaChargingSFXClip;

	// Bow Transforms
	public Transform[] TopBowZTransforms;
	public Transform[] TopBowYTransforms;
	public Transform[] BottomBowZTransforms;
	public Transform[] BottomBowYTransforms;

	// Bow Loading Variables
	public float TotalDrawDistance = 2.0f;
	public float DrawSpeed = 0.25f;
	public float BowDrawnDistance = 0;
	public float BowDrawnPercent = 0;
	private Vector3 localBarrelStartPos;
	private Vector3 localBarrelEndPos;
	private Vector3 localBarrelCurrentPos;
	// Top Rotation Eulers
	public float currentZRotation = 22;
	private Vector3 topCurrentZEulers;
	private Vector3 topCurrentYEulers;
	// Bottom Rotation Eulers
	public float MaxYDrawRotation = 15.0f;
	public float currentYRotation = 7.5f;
	private float startingYRotation = 0;
	private Vector3 bottomCurrentZEulers;
	private Vector3 bottomCurrentYEulers;

	// Ballista Bolt Variables
	public float BoltActivationSpeed = 0.5f;
	public GameObject BallistaBoltGO;
	private Transform ballistaBoltTransform;
	public Transform[] BallistaBoltPositions;
	public Transform[] BallistaBoltSections;
	public ParticleSystem BallistBoltFiringEffect01;
	private float boltEffect01StartLifetime = 0.65f;
	private float boltEffect01CurLifetime = 0.65f;
	public float BoltEffect01Lifetime = 2.0f;

	private float currentScale = 1.0f;

	void Awake () {
		myTransform = gameObject.transform;

		ballistaChargeSFXList = new List<AudioSource> ();

		// Scale Effects
		currentScale = myTransform.localScale.x;
		// Scale Effects
		boltEffect01CurLifetime = boltEffect01CurLifetime * currentScale;
		BoltEffect01Lifetime = BoltEffect01Lifetime * currentScale;
		if (BallistBoltFiringEffect01 != null) {
			BallistBoltFiringEffect01.startSpeed = BallistBoltFiringEffect01.startSpeed * currentScale;
			BallistBoltFiringEffect01.startSize = BallistBoltFiringEffect01.startSize * currentScale;
		}

		// Update Ballista Bolt
		if (BallistaBoltGO != null) {
			ballistaBoltTransform = BallistaBoltGO.transform;
			BallistaBoltGO.SetActive (false);
		}
		
		// Create Ballista Strings
		topBallistaStrings = new UTAW_BallistaString[TopStringPoints.Length];
		for (int i = 0; i < TopStringPoints.Length; i++) {
			string bStringName = "TopString_" + i.ToString ();
			topBallistaStrings[i] = CreateBallistaStrings (bStringName, TopStringPoints [i]);
			ballistaChargeSFXList.Add(TopStringPoints [i].gameObject.AddComponent<AudioSource> ());
		}
		bottomBallistaStrings = new UTAW_BallistaString[BottomStringPoints.Length];
		for (int i = 0; i < BottomStringPoints.Length; i++) {
			string bStringName = "BottomString_" + i.ToString ();
			bottomBallistaStrings[i] = CreateBallistaStrings (bStringName, BottomStringPoints [i]);
			ballistaChargeSFXList.Add(BottomStringPoints [i].gameObject.AddComponent<AudioSource> ());
		}

		// Setup Ballista Charge SFX Audio Sources
		if (ballistaChargeSFXList.Count > 0) {
			for (int i = 0; i < ballistaChargeSFXList.Count; i++) {
				ballistaChargeSFXList [i].playOnAwake = false;
				ballistaChargeSFXList [i].priority = 64;
				ballistaChargeSFXList [i].volume = 0.5f;
				ballistaChargeSFXList [i].spatialBlend = 1.0f;
				ballistaChargeSFXList [i].clip = BallistaChargingSFXClip;
			}	
			hasPlayedChargingSFX = false;
		}

		startingYRotation = currentYRotation;
		UpdateBowDrawRotations ();

//		// Setup Y Bow Eulers
//		topCurrentYEulers = TopBowYTransforms[0].localRotation.eulerAngles;
//		currentYRotation = topCurrentYEulers.y;
//		bottomCurrentYEulers = BottomBowYTransforms[0].localRotation.eulerAngles;
//		// Setup Z Bow Eulers
//		topCurrentZEulers = TopBowZTransforms[0].localRotation.eulerAngles;
//		currentZRotation = topCurrentZEulers.y;
//		bottomCurrentZEulers = BottomBowZTransforms[0].localRotation.eulerAngles;

		// Setup Loading Variables
		localBarrelStartPos = BarrelTransform.localPosition;
		localBarrelEndPos = BarrelTransform.localPosition;
		localBarrelEndPos.z -= TotalDrawDistance;
	}

	private UTAW_BallistaString CreateBallistaStrings (string bStringName, Transform bStringParent) {
		if (BallistaStringPrefab != null) {
			GameObject newStringGO = GameObject.Instantiate (BallistaStringPrefab, bStringParent.position, bStringParent.rotation) as GameObject;
			newStringGO.name = bStringName;
			newStringGO.transform.parent = bStringParent;
			UTAW_BallistaString stringScript = newStringGO.GetComponent<UTAW_BallistaString> ();
			stringScript.ScaleEffects (currentScale);
			return stringScript;
		} else {
			return null;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Update String States
		UpdateStringStates ();

		if (CurrentState == BallistaStateTypes.Inactive) {
			Inactive ();
			CurrentState = BallistaStateTypes.Loading;
		} else if (CurrentState == BallistaStateTypes.Loading) {
			if (!BallistaBoltGO.activeSelf)
				BallistaBoltGO.SetActive (true);
			LoadBallista ();
		} else if (CurrentState == BallistaStateTypes.Loaded) {
			if (TestFireBallista) {
				FireBallista ();
				TestFireBallista = false;
			}
		} else if (CurrentState == BallistaStateTypes.Fired) {
			// Bow Has Been Fired
			if (BallistaBoltGO.activeSelf) {
				BallistaBoltGO.SetActive (false);
				// Reset Bolt
				for (int i = 0; i < BallistaBoltSections.Length; i++) {
					BallistaBoltSections [i].localPosition = ballistaBoltTransform.localPosition;
				}
				boltEffect01CurLifetime = boltEffect01StartLifetime;
				if (BallistBoltFiringEffect01 != null) {
					BallistBoltFiringEffect01.startLifetime = boltEffect01StartLifetime;
				}
			}
			CurrentState = BallistaStateTypes.ReloadPause;
		} else if (CurrentState == BallistaStateTypes.ReloadPause) {
			if (BallistaChangingConfig) {
				if (CurrentBallistaConfig == BallistaBowTypes.Closed) {
					if (currentZRotation > 0.1f) {
						currentZRotation -= 25 * Time.deltaTime;
					} else {
						currentZRotation = 0;
						BallistaChangingConfig = false;
					}
				} else if (CurrentBallistaConfig == BallistaBowTypes.Open) {
					if (currentZRotation < 23.9f) {
						currentZRotation += 25 * Time.deltaTime;
					} else {
						currentZRotation = 24;
						BallistaChangingConfig = false;
					}
				}
			} else {
				// Bow Reload Between Fired and Drawing (Short Delay)
				if (reloadPauseTimer < ReloadPauseTime) {
					reloadPauseTimer += Time.deltaTime;
				} else {
					// Draw and Load Ballista
					hasPlayedChargingSFX = false;
					CurrentState = BallistaStateTypes.Loading;
					reloadPauseTimer = 0;
				}
			}
		}

		// Update Ballista Bolt
		if (BallistaBoltGO.activeSelf) {
			UpdateBallistaBolt ();	
		}

		// Update Bow Draw Rotations
		UpdateBowDrawRotations();
		// Update Barrel Local Position
		BarrelTransform.localPosition = localBarrelCurrentPos;

		UpdateLoadingLocations ();
		UpdateBallistaStrings ();
	}

	// State Update Functions
	private void Inactive () {
		if (TestFireBallista) {
			CurrentState = BallistaStateTypes.Loading;
			TestFireBallista = false;
		}
	}
	private void LoadBallista () {
		if (BowDrawnDistance < (TotalDrawDistance * 0.975f)) {
			if (!hasPlayedChargingSFX) {
				if (ballistaChargeSFXList.Count > 0) {
					for (int i = 0; i < ballistaChargeSFXList.Count; i++) {
						ballistaChargeSFXList [i].Play();
					}	
					hasPlayedChargingSFX = true;
				}
			}
			localBarrelCurrentPos.z -= DrawSpeed * Time.deltaTime; // = Vector3.LerpUnclamped (localBarrelCurrentPos, localBarrelEndPos, DrawSpeed * Time.deltaTime);
			if (boltEffect01CurLifetime < BoltEffect01Lifetime)
				boltEffect01CurLifetime += Time.deltaTime / 2;
			if (BallistBoltFiringEffect01 != null) {
				BallistBoltFiringEffect01.startLifetime = boltEffect01CurLifetime;
			}
		} else {
			localBarrelCurrentPos = localBarrelEndPos;
			CurrentState = BallistaStateTypes.Loaded;
		}
	}
	private void FireBallista () {
		localBarrelCurrentPos = localBarrelStartPos;
		CurrentState = BallistaStateTypes.Fired;
	}

	private void UpdateBallistaBolt () {
		if (CurrentState == BallistaStateTypes.Loading) {
			for (int i = 0; i < BallistaBoltPositions.Length; i++) {
				BallistaBoltSections [i].localPosition = Vector3.Lerp (BallistaBoltSections [i].localPosition, BallistaBoltPositions [i].localPosition, BoltActivationSpeed * Time.deltaTime);
			}
		}
	}
	private void UpdateStringStates() {
		if (topBallistaStrings.Length > 0) {
			for (int i = 0; i < topBallistaStrings.Length; i++) {
				topBallistaStrings [i].StringState = CurrentState;
			}
		}
		if (bottomBallistaStrings.Length > 0) {
			for (int i = 0; i < bottomBallistaStrings.Length; i++) {
				bottomBallistaStrings [i].StringState = CurrentState;
			}
		}
	}

	private void UpdateBowDrawRotations() {
		BowDrawnPercent = BowDrawnDistance / TotalDrawDistance;
		float bowDrawAddition = (MaxYDrawRotation - startingYRotation) * BowDrawnPercent;
		currentYRotation = startingYRotation + bowDrawAddition;

		topCurrentYEulers.y = currentYRotation;
		bottomCurrentYEulers.y = currentYRotation;
		topCurrentZEulers.z = currentZRotation;
		bottomCurrentZEulers.z = currentZRotation;

		// Update Top Rotations
		for (int i = 0; i < TopBowYTransforms.Length; i++) {
			if (i > 2)
				TopBowYTransforms [i].localRotation = Quaternion.Euler (topCurrentYEulers);
			else
				TopBowYTransforms [i].localRotation = Quaternion.Euler (-topCurrentYEulers);
		}
		for (int i = 0; i < TopBowZTransforms.Length; i++) {
			if (i > 0)
				TopBowZTransforms [i].localRotation = Quaternion.Euler (topCurrentZEulers);
			else
				TopBowZTransforms [i].localRotation = Quaternion.Euler (-topCurrentZEulers);
		}
		// Update Bottom Rotations
		for (int i = 0; i < BottomBowYTransforms.Length; i++) {
			if (i > 2)
				BottomBowYTransforms [i].localRotation = Quaternion.Euler (bottomCurrentYEulers);
			else
				BottomBowYTransforms [i].localRotation = Quaternion.Euler (-bottomCurrentYEulers);
		}
		for (int i = 0; i < BottomBowZTransforms.Length; i++) {
			if (i > 0)
				BottomBowZTransforms [i].localRotation = Quaternion.Euler (-bottomCurrentZEulers);
			else
				BottomBowZTransforms [i].localRotation = Quaternion.Euler (bottomCurrentZEulers);
		}
	}

	private void UpdateLoadingLocations () {
//		localBarrelCurrentPos = BarrelTransform.localPosition;
		BowDrawnDistance = Vector3.Distance (localBarrelStartPos, localBarrelCurrentPos);
	}

	private void UpdateBallistaStrings () {
		// Update Top Ballista Strings
		if (topBallistaStrings.Length > 0) {
			for (int i = 0; i < topBallistaStrings.Length; i++) {
				topBallistaStrings [i].UpdateBallistaString (TopStringConnectTransform.position, TopStringPoints [i].position, BowDrawnPercent);
			}
		}
		// Update Bottom Ballista Strings
		if (bottomBallistaStrings.Length > 0) {
			for (int i = 0; i < bottomBallistaStrings.Length; i++) {
				bottomBallistaStrings [i].UpdateBallistaString (BottomStringConnectTransform.position, BottomStringPoints [i].position, BowDrawnPercent);
			}
		}
	}
}
