using UnityEngine;
using System.Collections;

public class UTAW_BallistaString : MonoBehaviour {

	public BallistaStateTypes StringState = BallistaStateTypes.Inactive;

	public LineRenderer StringLineRenderer;
	public ParticleSystem StringTopEffect;
	public ParticleSystem StringBottomEffect;

	private float currentLineWidth = 0;
	public float MaxStringWidth = 0.5f;
	public float MaxTopEffectSize = 0.75f;
	public float MaxBottomEffectSize = 0.35f;

	// Top String Effect
	private bool hasTopStringEffect = false;
	private Transform topStringEffectTrans;

	// Bottom String Effect
	private bool hasBottomStringEffect = false;
	private Transform bottomStringEffectTrans;

	// Use this for initialization
	void Start () {
		if (StringTopEffect != null) {
			hasTopStringEffect = true;
			topStringEffectTrans = StringTopEffect.transform;
		} else {
			hasTopStringEffect = false;
		}
		if (StringBottomEffect != null) {
			hasBottomStringEffect = true;
			bottomStringEffectTrans = StringBottomEffect.transform;
		} else {
			hasBottomStringEffect = false;
		}
	}

	public void ScaleEffects(float parentScale) {
		MaxTopEffectSize = MaxTopEffectSize * parentScale;
		MaxBottomEffectSize = MaxBottomEffectSize * parentScale;
		MaxStringWidth = MaxStringWidth * parentScale;
		if (StringTopEffect != null) {
			StringTopEffect.startLifetime = StringTopEffect.startLifetime * parentScale;
		}
		if (StringBottomEffect != null) {
			StringBottomEffect.startLifetime = StringTopEffect.startLifetime * parentScale;
		}
	}

	public void UpdateBallistaString(Vector3 topConnectPos, Vector3 bottomConnectPos, float percentDrawn) {
		// Update Via StringState
		if (StringState == BallistaStateTypes.Inactive) {
			currentLineWidth = 0;
			if (StringTopEffect.isPlaying)
				StringTopEffect.Stop ();
			if (StringBottomEffect.isPlaying)
				StringBottomEffect.Stop ();
			StringTopEffect.startSize = 0;
			StringBottomEffect.startSize = 0;
		}
		else if (StringState == BallistaStateTypes.Loading) {
			if (!StringTopEffect.isPlaying)
				StringTopEffect.Play ();			
			if (!StringBottomEffect.isPlaying)
				StringBottomEffect.Play ();			
			StringTopEffect.startSize = MaxTopEffectSize * percentDrawn;
			StringBottomEffect.startSize = MaxBottomEffectSize * percentDrawn;
			currentLineWidth = MaxStringWidth * percentDrawn;
		}
		else if (StringState == BallistaStateTypes.ReloadPause) {
			if (StringTopEffect.isPlaying)
				StringTopEffect.Stop ();
			if (StringBottomEffect.isPlaying)
				StringBottomEffect.Stop ();
			if (currentLineWidth > 0)
				currentLineWidth -= Time.deltaTime / 2;
			else
				currentLineWidth = 0;
		}

		// Update Positions
		if (hasTopStringEffect) {
			topStringEffectTrans.position = topConnectPos;
			topStringEffectTrans.LookAt (bottomConnectPos);
		}
		if (hasBottomStringEffect) {
			bottomStringEffectTrans.position = bottomConnectPos;
			bottomStringEffectTrans.LookAt (topConnectPos);
		}
		// Update LineRenderer
		if (StringLineRenderer != null) {
			StringLineRenderer.SetWidth (currentLineWidth, currentLineWidth);
			StringLineRenderer.SetPosition (0, topConnectPos);
			StringLineRenderer.SetPosition (1, bottomConnectPos);
		}
	}
}
