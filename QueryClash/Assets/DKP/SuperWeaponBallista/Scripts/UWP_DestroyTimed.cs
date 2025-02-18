using UnityEngine;
using System.Collections;

public class UWP_DestroyTimed : MonoBehaviour {
	public float LifeTime = 20;
	private float timer = 0;

	public Light LightToFlash;

	void Awake () {
		if (LightToFlash != null) {
			LightToFlash.intensity = 0;
		}
	}

	// Update is called once per frame
	void Update () {
		if (timer < LifeTime) {
			timer += Time.deltaTime;

			if (LightToFlash != null) {
				if (timer < LifeTime / 2) {
					if (LightToFlash.intensity < 1.0f) {
						LightToFlash.intensity += Time.deltaTime;
					}
				}
				else if (timer >= LifeTime / 2) {
					if (LightToFlash.intensity > 0f) {
						LightToFlash.intensity -= Time.deltaTime;
					}
				}
			}

		} else {
			Destroy (gameObject);
		}
	}
}
