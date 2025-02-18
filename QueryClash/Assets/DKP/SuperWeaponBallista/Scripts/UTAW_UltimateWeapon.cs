using UnityEngine;
using System.Collections;

public enum UTAW_UltimateWeaponTypes {
	Projectile,
	Missile,
	Beam,
	Ballista
}

public class UTAW_UltimateWeapon : MonoBehaviour {

	public UTAW_UltimateWeaponTypes WeaponType = UTAW_UltimateWeaponTypes.Projectile;

	public GameObject ProjectilePrefab;
	public GameObject MuzzleFlashPrefab;

	public Transform BarrelTransform;
	public Transform FirePointTransform;
	public Transform ExhaustPortTransform;

	public Transform BarrelZRotationTransform;
	public float ZSpinSpeed = 2.5f;
	public bool RotateOpposite = false;
	private float zSpinTimer = 0;

	private bool projectileIsRocket = false;

	// Barrel Exhaust
	private ParticleSystem BarrelExhaustEffect;
	public GameObject BarrelExhaustPrefab;

	private float reloadTimer = 0;
	public float ReloadTime = 2.0f;
	public bool Reloaded = false;

	private Vector3 barrelStartingLocPos;
	private Vector3 barrelFireLocPos;

	public float BarrelReturnSpeed = 2.0f;
	public float BarrelKickbackDistance = 0.35f;

	public AudioSource BarrelFiringSFX;
	private GameObject activeMuzzleFlash;
	private Transform activeMuzzleFlashTranform;
	public float MuzzleFlashTime = 0.25f;
	private float muzzleFlashTimer = 0;
	public bool RandomizeMuzzle = false;
	public float PercentToRandomize = 0.25f;
	private Vector3 muzzleLocalScale;
	private float muzzleOrigXScale = 1.0f;
	private float muzzleMinXScale = 0.5f;
	private float muzzleMaxXScale = 1.0f;
	private float muzzleOrigYScale = 1.0f;
	private float muzzleMinYScale = 0.5f;
	private float muzzleMaxYScale = 1.0f;
	private float muzzleOrigZScale = 1.0f;
	private float muzzleMinZScale = 0.5f;
	private float muzzleMaxZScale = 1.0f;

	// Charge Effects
	public GameObject[] ChargingEffectGOs;
	private bool chargingEffectsActive = true;
	public Transform ChargingXRotationTransform;
	private Vector3 chargingXEulers;
	private Vector3 chargingXChargedEulers;
	private float totalDistance = 0;
	public float XRotationCharged = -22;
	public float XRotationFired = 15;

	// Ballista Weapon Variables
	private UTAW_UltimateBallista ballistaWeaponScript;

	void Awake () {
		// Disable Charging Effects
		if (ChargingXRotationTransform != null) {
			chargingXEulers = new Vector3 (XRotationFired, 0, 0);
			chargingXChargedEulers = new Vector3 (XRotationCharged, 0, 0);
			totalDistance = Vector3.Distance (chargingXEulers, chargingXChargedEulers);
			ChargingXRotationTransform.localRotation = Quaternion.Euler (chargingXEulers);
		}
		DisableChargingEffects();
	}

	// Use this for initialization
	void Start () {
		
		// Ballista Weapon Check
		if (WeaponType == UTAW_UltimateWeaponTypes.Ballista) {
			ballistaWeaponScript = gameObject.GetComponent<UTAW_UltimateBallista> ();
		}

		// Setup Barrel Kickback Positions
		barrelStartingLocPos = BarrelTransform.localPosition;

		Vector3 offsetPos = BarrelTransform.localPosition;
		offsetPos.z -= BarrelKickbackDistance;
		barrelFireLocPos = offsetPos;

		// Create Muzzle Flash GameObject
		if (MuzzleFlashPrefab != null) {
			if (activeMuzzleFlash == null) {
				GameObject newFlashGO = GameObject.Instantiate (MuzzleFlashPrefab, FirePointTransform.position, FirePointTransform.rotation) as GameObject;
				newFlashGO.transform.parent = FirePointTransform;
				newFlashGO.SetActive (false);
				muzzleFlashTimer = 0;
				activeMuzzleFlash = newFlashGO;
				activeMuzzleFlashTranform = activeMuzzleFlash.transform;
				muzzleLocalScale = activeMuzzleFlashTranform.localScale;
				muzzleOrigXScale = muzzleLocalScale.x;
				muzzleOrigYScale = muzzleLocalScale.y;
				muzzleOrigZScale = muzzleLocalScale.z;

				muzzleMinXScale = muzzleLocalScale.x - (muzzleLocalScale.x * PercentToRandomize);
				muzzleMaxXScale = muzzleLocalScale.x + (muzzleLocalScale.x * PercentToRandomize);
				muzzleMinYScale = muzzleLocalScale.y - (muzzleLocalScale.y * PercentToRandomize);
				muzzleMaxYScale = muzzleLocalScale.y + (muzzleLocalScale.y * PercentToRandomize);
				muzzleMinZScale = muzzleLocalScale.z - (muzzleLocalScale.z * PercentToRandomize);
				muzzleMaxZScale = muzzleLocalScale.z + (muzzleLocalScale.z * PercentToRandomize);
			}
		}

		// Create Barrel Exhaust
		if (BarrelExhaustPrefab != null) {
			GameObject newExhaustGO = GameObject.Instantiate (BarrelExhaustPrefab, ExhaustPortTransform.position, ExhaustPortTransform.rotation) as GameObject;
			newExhaustGO.transform.parent = ExhaustPortTransform;
			BarrelExhaustEffect = newExhaustGO.GetComponent<ParticleSystem> ();
		}
	}
	
	// Update is called from manager
	void Update () {
		// Update MuzzleFlashes
		if (activeMuzzleFlash != null) {
			UpdateMuzzleFlash ();
		}

		// Update Z Rotation
		UpdateZRotation();

		// Reload Turret - Turret Reloads Even if doesn't have a target
		if (WeaponType != UTAW_UltimateWeaponTypes.Ballista) {
			if (!Reloaded) {
				if (reloadTimer < ReloadTime) {
					if (!chargingEffectsActive)
						EnableChargingEffects ();
					if (ChargingXRotationTransform != null)
						UpdateChargingRotation ();
					reloadTimer += Time.deltaTime;
					if (BarrelKickbackDistance > 0) {
						BarrelTransform.localPosition = Vector3.Lerp (BarrelTransform.localPosition, barrelStartingLocPos, BarrelReturnSpeed * Time.deltaTime);
					}
				} else {
					if (chargingEffectsActive)
						DisableChargingEffects ();
					Reloaded = true;
				}
			}
		} else {
			// Ballista
			if (ballistaWeaponScript != null) {
				if (ballistaWeaponScript.CurrentState == BallistaStateTypes.Loaded) {
					Reloaded = true;
				} else {
					Reloaded = false;
				}
			}
		}
	}

	private void UpdateChargingRotation() {		
		float chargingPercent = reloadTimer / ReloadTime;
		float distanceToCompleteCharge = Vector3.Distance (chargingXEulers, chargingXChargedEulers);
		float amountRotated = distanceToCompleteCharge / totalDistance;
		chargingXEulers = Vector3.Lerp (chargingXEulers, chargingXChargedEulers, amountRotated * chargingPercent);
		ChargingXRotationTransform.localRotation = Quaternion.Euler (chargingXEulers);
	}

	private void DisableChargingEffects() {
		if (ChargingEffectGOs.Length > 0) {
			for (int i = 0; i < ChargingEffectGOs.Length; i++) {
				ChargingEffectGOs [i].SetActive (false);
			}
		}
		chargingEffectsActive = false;
	}

	private void EnableChargingEffects() {
		if (ChargingEffectGOs.Length > 0) {
			for (int i = 0; i < ChargingEffectGOs.Length; i++) {
				ChargingEffectGOs [i].SetActive (true);
			}
		}
		chargingEffectsActive = true;
	}

	private void UpdateMuzzleFlash() {
		if (activeMuzzleFlash != null) {
			if (muzzleFlashTimer > 0) {				
				muzzleFlashTimer -= Time.deltaTime;
			} else {
				if (activeMuzzleFlash != null) {
					if (activeMuzzleFlash.activeSelf)
						activeMuzzleFlash.SetActive (false);
				}
			}
		}
	}

	private void UpdateZRotation() {
		if (BarrelZRotationTransform != null) {
			if (zSpinTimer > 0) {				
				zSpinTimer -= Time.deltaTime;
				if (RotateOpposite) {
					BarrelZRotationTransform.RotateAround (BarrelZRotationTransform.position, BarrelZRotationTransform.forward, -(ZSpinSpeed * Time.deltaTime));
				} else {
					BarrelZRotationTransform.RotateAround (BarrelZRotationTransform.position, BarrelZRotationTransform.forward, ZSpinSpeed * Time.deltaTime);
				}
			}
		}
	}

	public void FireWeapon (Transform currentTargetIn) {
		if (Reloaded) {
			// Fire Ballista
			if (ballistaWeaponScript != null) {
				ballistaWeaponScript.TestFireBallista = true;
			}
			// Reset Charging X Rotation
			if (ChargingXRotationTransform != null) {
				chargingXEulers = new Vector3 (XRotationFired, 0, 0);
				ChargingXRotationTransform.localRotation = Quaternion.Euler (chargingXEulers);
			}
			// Activate MuzzleFlash
			if (activeMuzzleFlash != null) {
				muzzleFlashTimer = MuzzleFlashTime;
				if (!activeMuzzleFlash.activeSelf) {
					// Randomize Scale
					if (RandomizeMuzzle) {
						muzzleLocalScale.x = Random.Range (muzzleMinXScale, muzzleMaxXScale);
						muzzleLocalScale.y = Random.Range (muzzleMinYScale, muzzleMaxYScale);
						muzzleLocalScale.z = Random.Range (muzzleMinZScale, muzzleMaxZScale);
						activeMuzzleFlashTranform.localScale = muzzleLocalScale;
					}
					activeMuzzleFlash.SetActive (true);
				}
			}
			// Spin Z Rotation Transform
			if (BarrelZRotationTransform != null) {
				zSpinTimer = ReloadTime;
			}
			// Play Barrel Firing SFX
			if (BarrelFiringSFX != null) {
				BarrelFiringSFX.Play ();
			}
			if (BarrelExhaustEffect != null) {
				BarrelExhaustEffect.Play ();
			}

			// Spawn Projectile
			if (ProjectilePrefab != null) {
				BarrelTransform.localPosition = barrelFireLocPos;
				Vector3 startingPosition = FirePointTransform.position;
				Quaternion startingRotation = FirePointTransform.rotation;
				GameObject newProjectile = GameObject.Instantiate (ProjectilePrefab, startingPosition, startingRotation) as GameObject;
			}

			reloadTimer = 0;
			Reloaded = false;
		}
	}
}
