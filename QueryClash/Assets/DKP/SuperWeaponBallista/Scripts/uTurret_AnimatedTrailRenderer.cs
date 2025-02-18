using UnityEngine;
using System.Collections;

public class uTurret_AnimatedTrailRenderer : MonoBehaviour {
	private TrailRenderer myTrailRenerer;

	// Use this for initialization
	void Start () {
		myTrailRenerer = gameObject.GetComponent<TrailRenderer> ();
		if (myTrailRenerer == null) {
			Debug.LogError ("No Trail Renderer Found!");
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (myTrailRenerer != null) {
			AnimateTexture ();
		}
	}

	public int AnimationTileX = 4;
	public int AnimationTileY = 4;
	public float FramesPerSecond = 30.0f;

	private void AnimateTexture() {
		// Calculate Index
		int index = (int)(Time.time * FramesPerSecond);
		// Repeat When Exhaust All Frames
		index = index % (AnimationTileX * AnimationTileY);

		// Size of every time
		Vector2 size = new Vector2(1.0f / AnimationTileX, 1.0f / AnimationTileX);

		// Split into Horz and Vert Indexes
		int uIndex = index % AnimationTileX;
		int vIndex = index / AnimationTileX;

		// Build Offset
		Vector2 offset = new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y);

		myTrailRenerer.sharedMaterial.SetTextureOffset("_MainTex", offset);
		myTrailRenerer.sharedMaterial.SetTextureScale("_MainTex", size);
	}
}
