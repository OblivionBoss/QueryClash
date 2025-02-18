using UnityEngine;
using System.Collections;

public enum UTAW_TurretTargetingModes {
	AutomatedTargeting,
	ManuallyControlled
}

public class UTAW_TurretController : MonoBehaviour {

	private Transform myTransform;

	public int FactionNumber = 1;
	public UTAW_TurretTargetingModes CurrentTargetMode = UTAW_TurretTargetingModes.AutomatedTargeting;
	public Transform ManualTargetTransform;
	public bool UseLoS = true;

	public UTAW_UltimateWeapon[] TurretWeapons;

	// Turret Setup Transforms
	public Transform TurretRotationTransform;
	public Transform TurretYRotationTransform;
	private Vector3 startingRotation;
	private float timeWithoutTarget = 0;
	private float timeWithoutTargetFreq = 1.0f;
	public float TurretRotationSpeed = 2.5f;
	public float LeadTargetAmount = 1.0f;

	public bool Activated = true;
	public bool DelayActivation = true;
	private float delayActiveTimer = 0;
	private float delayActiveTimerFreq = 0.5f;

	// Barrels
	public int NumberOfBarrels = 0;
	public int CurrentBarrel = 0;

	private Vector3[] barrelStartingLocPos;
	private Vector3[] barrelFireLocPos;

	public float BarrelReloadTimer = 0;
	public float TimeBetweenBarrelFire = 0.5f;

	// Target Aquisition
	public float TargetRangeMAX = 120;
	public float DistanceToTarget = 0;
	public Transform CurrentTarget;
	private Vector3 lastTargetPosition;
	private Vector3 currentTargetPosition;
	public float TargetSpeed = 0;
	private float targetAquisitionTimer = 0;
	private float ScanForTargetTime = 1.0f;
	public bool WeaponFiringOverride = false;

	// Fire All Barrels At Once
	public bool BarrelsAreLinked = false;
	public GameObject LinkedProjectilePrefab;
	public GameObject LinkedMuzzleBlastPrefab;
	public AudioSource LinkedFireSFX;
	private GameObject activeMussleBlast;
	public Transform LinkedFiringPoint;
	public bool FireAllBarrelsAtOnce = false;
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

	void Awake () {
		if (DelayActivation) {
			Activated = false;
			// Setup Random Activation Delay
			delayActiveTimerFreq = Random.Range(0.5f, 1.5f);

		} else {
			Activated = true;
		}
	}

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;

		// Find all Weapon Barrels on Turret
		TurretWeapons = gameObject.GetComponentsInChildren<UTAW_UltimateWeapon>();
		NumberOfBarrels = TurretWeapons.Length;

		// Get Our Faction Number if Attached to Starship
		uTurret_Target parentTargetScript = transform.root.gameObject.GetComponent<uTurret_Target>();
		if (parentTargetScript != null) {
			FactionNumber = parentTargetScript.FactionNumber;
		}

		// Setup Default Rotation
		startingRotation = TurretRotationTransform.forward;

		// Create Linked Muzzle Flash GameObject
		if (LinkedMuzzleBlastPrefab != null) {
			if (activeMuzzleFlash == null) {
				GameObject newFlashGO = GameObject.Instantiate (LinkedMuzzleBlastPrefab, LinkedFiringPoint.position, LinkedFiringPoint.rotation) as GameObject;
				newFlashGO.transform.parent = LinkedFiringPoint;
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
	}
	
	// Update is called once per frame
	void Update () {
		if (!Activated) {
			if (delayActiveTimer < delayActiveTimerFreq) {
				delayActiveTimer += Time.deltaTime;
			} else {
				Activated = true;
			}
		}

		if (Activated) {

			// Update MuzzleFlashes
			if (activeMuzzleFlash != null) {
				UpdateMuzzleFlash ();
			}

			// Automated Targeting Mode Update
			if (CurrentTargetMode == UTAW_TurretTargetingModes.AutomatedTargeting) {
				if (ActiveCurrentTarget == null) {
					if (CurrentTarget == null) {
						timeWithoutTarget += Time.deltaTime;

						// Scan For New Target
						if (timeWithoutTarget > timeWithoutTargetFreq) {
//					Debug.Log ("Scanning for Target...");
							CurrentTarget = uTurrets_TargetManager.GlobalAccess.GetActiveTarget (TurretRotationTransform, TargetRangeMAX, FactionNumber);
							ActiveCurrentTarget = CurrentTarget;
							ProcessTarget ();
							timeWithoutTarget = 0;
						}
					} else {
						ActiveCurrentTarget = CurrentTarget;
					}
				}			

				// Scan For Alternative Sub Target
				if (ActiveCurrentTarget != null) {
					if (!WeaponFiringOverride) {
						ScanTargetForNewActive ();
					}
				}
			

				// Check Range and LOS To Target
				if (ActiveCurrentTarget != null) {
					timeWithoutTarget = 0;
					// Check LOS
					CheckLOSOnTarget ();
					if (!HasLOSOnTarget) {
						ActiveCurrentTarget = null;
					}
					// If Pass LOS Check, check distance
					if (ActiveCurrentTarget != null) {
						DistanceToTarget = Vector3.Distance (TurretRotationTransform.position, ActiveCurrentTarget.position);
						if (DistanceToTarget > TargetRangeMAX) {
							ActiveCurrentTarget = null;
						}
					}
				}
			

				// Rotate Towards ActiveTarget
				if (ActiveCurrentTarget != null) {
					// Calculate Target Speed
					currentTargetPosition = ActiveCurrentTarget.position;
					TargetSpeed = Vector3.Distance (lastTargetPosition, currentTargetPosition);
					float distanceToTarget = Vector3.Distance (myTransform.position, ActiveCurrentTarget.position);
					LeadTargetAmount = TargetSpeed * distanceToTarget;

					// Lead Target
					Vector3 targetCurrentPos = ActiveCurrentTarget.position;
					Vector3 movementDirection = currentTargetPosition - lastTargetPosition;
					targetCurrentPos = targetCurrentPos + (movementDirection * LeadTargetAmount);

					Quaternion targetLookRotation = Quaternion.LookRotation (targetCurrentPos - myTransform.position);
					TurretRotationTransform.rotation = Quaternion.Lerp (TurretRotationTransform.rotation, targetLookRotation, TurretRotationSpeed * Time.deltaTime);

					// Update Turret Y Rotation
					if (TurretYRotationTransform != null) {
						Vector3 yEulers = new Vector3 (0, TurretRotationTransform.rotation.eulerAngles.y, 0);
						TurretYRotationTransform.localRotation = Quaternion.Euler (yEulers);
					}

					lastTargetPosition = ActiveCurrentTarget.position;
				}

				// Fire At Target
				if (ActiveCurrentTarget != null) {
			
					// Fire Turret
					if (BarrelReloadTimer < TimeBetweenBarrelFire) {
						BarrelReloadTimer += Time.deltaTime;
					} else {
						// Double Check LoS
						if (CurrentTarget != null && CheckBarrelLOSTarget ()) {
							if (FireAllBarrelsAtOnce) {
								// Fire All Barrels At Once
								for (int i = 0; i < TurretWeapons.Length; i++) {
									if (TurretWeapons [i].Reloaded) {
										TurretWeapons [i].FireWeapon (CurrentTarget);
									}
								}
								// We fire a projectile (Linked Barrels)
								if (BarrelsAreLinked) {
									if (LinkedProjectilePrefab != null) {
										if (LinkedFiringPoint != null) {
											FireLinkedProjectile ();
										}
									}
								}
							} else {
								// Fire Current Barrel
								if (TurretWeapons [CurrentBarrel].Reloaded) {
									TurretWeapons [CurrentBarrel].FireWeapon (CurrentTarget);
								}
							}
						}
						if (CurrentBarrel < NumberOfBarrels - 1) {
							CurrentBarrel++;
						} else {
							// All Barrels Fired
							CurrentBarrel = 0;
						}
						BarrelReloadTimer = 0;
					}

				} else {
					if (CurrentBarrel != 0) {
						CurrentBarrel = 0;
					}
				}

				// No Target Return to Default Rotation
				if (ActiveCurrentTarget == null) {
					if (timeWithoutTarget < timeWithoutTargetFreq) {
						timeWithoutTarget += Time.deltaTime;
					} else {
						TurretRotationTransform.rotation = Quaternion.Lerp (TurretRotationTransform.rotation, Quaternion.LookRotation (startingRotation), (TurretRotationSpeed / 2) * Time.deltaTime);
					}
				}

			} else if (CurrentTargetMode == UTAW_TurretTargetingModes.ManuallyControlled) {
				// Manually Controlled Targeting

				// Rotate Towards Manual Target
				if (ManualTargetTransform != null) {
					// Calculate Target Speed
					currentTargetPosition = ManualTargetTransform.position;
					TargetSpeed = Vector3.Distance (lastTargetPosition, currentTargetPosition);
					float distanceToTarget = Vector3.Distance (myTransform.position, ManualTargetTransform.position);
					LeadTargetAmount = TargetSpeed * distanceToTarget;

					// Lead Target
					Vector3 targetCurrentPos = ManualTargetTransform.position;
					Vector3 movementDirection = currentTargetPosition - lastTargetPosition;
					targetCurrentPos = targetCurrentPos + (movementDirection * LeadTargetAmount);

					Vector3 lookRotation = targetCurrentPos - myTransform.position;
					if (lookRotation == Vector3.zero) {
						
					} else {
						Quaternion targetLookRotation = Quaternion.LookRotation (lookRotation);
						TurretRotationTransform.rotation = Quaternion.Lerp (TurretRotationTransform.rotation, targetLookRotation, TurretRotationSpeed * Time.deltaTime);
					}

					// Update Turret Y Rotation
					if (TurretYRotationTransform != null) {
						Vector3 yEulers = new Vector3 (0, TurretRotationTransform.rotation.eulerAngles.y, 0);
						TurretYRotationTransform.localRotation = Quaternion.Euler (yEulers);
					}

					lastTargetPosition = ManualTargetTransform.position;
				}
			}
		}
	}

	public void TryManualFiringTurret () {
		// Fire Turret
		if (BarrelReloadTimer < TimeBetweenBarrelFire) {
			BarrelReloadTimer += Time.deltaTime;
		} else {
			// Double Check LoS
			if (ManualTargetTransform != null) {
				if (FireAllBarrelsAtOnce) {
					// Fire All Barrels At Once
					for (int i = 0; i < TurretWeapons.Length; i++) {
						if (TurretWeapons [i].Reloaded) {
							TurretWeapons [i].FireWeapon (CurrentTarget);
						}
					}
					// We fire a projectile (Linked Barrels)
					if (BarrelsAreLinked) {
						if (LinkedProjectilePrefab != null) {
							if (LinkedFiringPoint != null) {
								FireLinkedProjectile ();
							}
						}
					}
				} else {
					// Fire Current Barrel
					if (TurretWeapons [CurrentBarrel].Reloaded) {
						TurretWeapons [CurrentBarrel].FireWeapon (CurrentTarget);
					}
				}
			}
			if (CurrentBarrel < NumberOfBarrels - 1) {
				CurrentBarrel++;
			} else {
				// All Barrels Fired
				CurrentBarrel = 0;
			}
			BarrelReloadTimer = 0;
		}
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

	// Sub Targeting
	private uTurret_Target activeTargetScript;
	public Transform ActiveCurrentTarget;
	private Transform CurrentSubTarget;
	private float scanForSubTimer = 0;
	private float scanForSubTimerFreq = 5.0f;
	private void ProcessTarget() {
		if (CurrentTarget != null) {
			activeTargetScript = CurrentTarget.gameObject.GetComponent<uTurret_Target> ();
		}
	}
	private void ScanTargetForNewActive() {
		if (scanForSubTimer < scanForSubTimerFreq) {
			scanForSubTimer += Time.deltaTime;
		} else {
//			Debug.Log ("Scanning for Sub Target... Turret Rotational.");
			ActiveCurrentTarget = activeTargetScript.GetActiveTargetTransform (TurretRotationTransform, CurrentTarget);
			scanForSubTimer = 0;
		}
	}

	// Line of Sight Check
	public bool HasLOSOnTarget = false;
	private Ray losRay;
	private RaycastHit rayHit;
	private float losTimer = 0;
	private float losTimerFreq = 0.25f;
	private void CheckLOSOnTarget() {
		// Use Rotation Transform for Line Of Sight Origin
		if (losTimer < losTimerFreq) {
			losTimer += Time.deltaTime;
		} else {
//			Debug.Log ("Checking LOS");
			Vector3 targetDirection = ActiveCurrentTarget.position - TurretRotationTransform.position;
			losRay = new Ray (TurretRotationTransform.position, targetDirection);
			if (Physics.Raycast (losRay, out rayHit, TargetRangeMAX + 10)) {
				if (rayHit.collider.transform.root.transform == CurrentTarget) {
					HasLOSOnTarget = true;
				} else {
//					Debug.Log ("LOS Hit: " + rayHit.collider.transform.root.transform.name);
					HasLOSOnTarget = false;
				}
			}

			losTimer = 0;
		}
	}
	private bool CheckBarrelLOSTarget() {
		// Use Rotation Transform for Line Of Sight Origin
		if (TurretWeapons.Length > 0) {
			Vector3 targetDirection = ActiveCurrentTarget.position - TurretWeapons [CurrentBarrel].FirePointTransform.position;
			losRay = new Ray (TurretWeapons [CurrentBarrel].FirePointTransform.position, targetDirection);
			if (Physics.Raycast (losRay, out rayHit, TargetRangeMAX + 10)) {
				if (rayHit.collider.transform.root.transform == CurrentTarget) {
					return true;
				} else {
//					Debug.Log ("LOS Hit: " + rayHit.collider.transform.root.transform.name);
					return false;
				}
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	// Fire Linked Projectile
	private void FireLinkedProjectile() {
		// Play Linked Firing SFX
		if (LinkedFireSFX != null) {
			LinkedFireSFX.Play ();
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

		if (LinkedProjectilePrefab != null) {
			Vector3 startingPosition = LinkedFiringPoint.position;
			Quaternion startingRotation = LinkedFiringPoint.rotation;
			GameObject newProjectile = GameObject.Instantiate (LinkedProjectilePrefab, startingPosition, startingRotation) as GameObject;
		}
	}
}
