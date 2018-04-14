using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardScript : MonoBehaviour {
	public Light glow;
	public Color startGlowColor;
	public Color endGlowColor;
	public float startGlowSize;
	public float endGlowSize;

	public bool EvaluateDistance(Vector3 pointerPosition, float senseDistance, float pickupDistance) {
		float distance = Mathf.Clamp(Vector3.Distance (transform.position, pointerPosition), pickupDistance, senseDistance);
		float percentage = Mathf.InverseLerp (pickupDistance, senseDistance, distance);
		glow.color = Color.Lerp (endGlowColor, startGlowColor, percentage);
		glow.range = Mathf.Lerp (endGlowSize, startGlowSize, percentage);
		return (distance <= pickupDistance);
		
	}
}
