using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGesture : MonoBehaviour {

	float handVelocity;
	Vector3 currentHandPos, lastHandPos;
	private Vector3 currentHandVelocity, lastHandVelocity;
	[HideInInspector]
	public float currentHandVelocityMagnitude;
	[Range(0.01f, 0.1f)]
	public float handBoomThreshold;
	float timeSinceHandBoom;
	public float minTimeBetweenHandBooms;
	Transform boom;
	ParticleSystem boomParticles;
	ParticleSystem handParticles;
	public float colorMin, colorMax;
	public Color color;

	void Start() {
		lastHandPos = transform.position;
		lastHandVelocity = Vector3.zero;
		boom = transform.Find("Boom");
		print("#" + boom);
		boomParticles = boom.gameObject.GetComponent<ParticleSystem>();
		handParticles = transform.gameObject.GetComponent<ParticleSystem>();
	}

	void Update() {
		currentHandPos = transform.position;
		currentHandVelocity = currentHandPos - lastHandPos;
		currentHandVelocityMagnitude = currentHandVelocity.magnitude;
		float velocityChange = Vector3.Distance(lastHandVelocity, currentHandVelocity);
		//print(velocityChange);
		if (velocityChange > handBoomThreshold) {
			handBoom();
		}
		lastHandPos = currentHandPos;
		lastHandVelocity = currentHandVelocity;

		if (currentHandVelocityMagnitude > 0.01f) {
			//handParticles.Play();
		}
		else {
			handParticles.Stop();
		}

	}

	private void handBoom() {
		timeSinceHandBoom += Time.deltaTime;
		if (timeSinceHandBoom < minTimeBetweenHandBooms) return;
		timeSinceHandBoom = 0;

		boom.transform.eulerAngles = World.camera.eulerAngles;
		boomParticles.startColor = color;
		boomParticles.Play();
	}

}
