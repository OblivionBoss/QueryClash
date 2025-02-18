using UnityEngine;
using System.Collections;

public enum UTAW_UltimateProjectileTypes {
	LaserBolt_Red
}

public class UTAW_UltimateProjectile : MonoBehaviour {

	public UTAW_UltimateProjectileTypes ProjType = UTAW_UltimateProjectileTypes.LaserBolt_Red;
	public GameObject SpawnOnDestroyPrefab;
	public LayerMask layerMask;

	public TrailRenderer ProjectileTrail;
	private Transform trailTransform;
	private float timeToLingerTrail;

	#region Damage
	// Damage Variables

	public float WeaponDamage = 10;
	void UpdateWeaponDamage(float damageIn) {
		WeaponDamage = damageIn;
	}

	#endregion

	private Transform myTransform;
	private RaycastHit hitPoint;

	bool isHit = false;
	bool isEffectSpawned = false;

	// Delay Repooling
	public bool DelayRepool = false;
	// Projectile LifeTime
	public float ProjectileLifeTime = 5f;
	// Delay to Re-Pool Projectile (ms)
	public float RepoolDelay;
	// Projectile Velocity
	public float ProjectileVelocity = 300f;
	// Raycast Prediction Muliplier
	public float RaycastPrediction = 2f;

	// Array of Additional Particles
	public ParticleSystem[] AdditionalParticles;
	// Array of Projectile Particles
	private ParticleSystem[] myParticles;

	// Projectile timer
	private float projectileTimer = 0f;                          

	void Awake()
	{
		// Cache transform and get all particle systems attached
		myTransform = GetComponent<Transform>();
		myParticles = GetComponentsInChildren<ParticleSystem>();
		if (ProjectileTrail != null) {
			trailTransform = ProjectileTrail.transform;
			timeToLingerTrail = ProjectileTrail.time + 0.1f;
		}
	}

	// OnActivated called by WeaponPoolManager 
	public void OnActivated()
	{
		// Reset flags and raycast structure
		isHit = false;
		isEffectSpawned = false;
		projectileTimer = 0f;
		hitPoint = new RaycastHit();
	}

	// OnDeactivated called by WeaponPoolManager 
	public void OnDeactivated()
	{          
		
	}

	// Stop attached particle systems emission and allow them to fade out before despawning
	void Delay()
	{       
		if(myParticles.Length > 0 && AdditionalParticles.Length > 0)
		{
			bool delayed;

			for (int i = 0; i < myParticles.Length; i++)
			{
				delayed = false;

				for (int y = 0; y < AdditionalParticles.Length; y++)                
					if (myParticles[i] == AdditionalParticles[y])
					{
						delayed = true;
						break;
					}                

				myParticles[i].Stop(false);

				if (!delayed)
					myParticles[i].Clear(false);                
			}
		}
	}

	// OnDespawned called by pool manager 
	void OnProjectileDestroy()
	{   
		// WeaponPoolManager Deactive
		if (ProjectileTrail != null) {
			trailTransform.parent = null;
			Destroy (trailTransform.gameObject, timeToLingerTrail);
		}	
		Destroy(gameObject);
	}

	// Apply hit force on impact
	void ApplyForce(float force)
	{
//		if (hitPoint.rigidbody != null)
//			hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
	}

	private void SpawnHitEffect(Vector3 hitPosition) {
		if (SpawnOnDestroyPrefab != null) {
			GameObject newExplosion = GameObject.Instantiate (SpawnOnDestroyPrefab, myTransform.position, myTransform.rotation) as GameObject;
		}
	}

	void Update()
	{
		// If something was hit
		if (isHit)
		{
			// Execute once
			if (!isEffectSpawned)
			{
				// Invoke corresponding method that spawns FX
				switch (ProjType)
				{
				case UTAW_UltimateProjectileTypes.LaserBolt_Red:
					SpawnHitEffect(hitPoint.point + hitPoint.normal * 0.2f);
					ApplyForce(2.5f);
					break;
				default:
					break;
				}
				isEffectSpawned = true;
			}

			// Deactivate Current Projectile 
			if(!DelayRepool || (DelayRepool && (projectileTimer >= RepoolDelay)))
				OnProjectileDestroy();
		}

		// No collision occurred yet
		else
		{
			// Projectile step per frame based on velocity and time
			Vector3 step = transform.forward * Time.deltaTime * ProjectileVelocity;

			// Raycast for targets with ray length based on frame step by ray cast advance multiplier
			if (Physics.Raycast(transform.position, transform.forward, out hitPoint, step.magnitude * RaycastPrediction, layerMask))
			{
				isHit = true;

//				TDDamageCatcher damageCatcher = hitPoint.collider.gameObject.GetComponent<TDDamageCatcher> ();
//				if (damageCatcher != null) {
//					damageCatcher.DamageFromWeapon (weaponDamage);
//				} else
				hitPoint.collider.gameObject.transform.root.gameObject.SendMessage ("Damage", WeaponDamage, SendMessageOptions.DontRequireReceiver);

				// Invoke delay routine if required
				if (DelayRepool)
				{
					// Reset projectile timer and let particles systems stop emitting and fade out correctly
					projectileTimer = 0f;                    
					Delay();
				}
			}
			// Nothing hit
			else
			{
				// Projectile despawn after run out of time
				if (projectileTimer >= ProjectileLifeTime)
					OnProjectileDestroy();
			}

			// Advances projectile forward
			transform.position += step;
		}

		// Updates projectile timer
		projectileTimer += Time.deltaTime;
	}

}
