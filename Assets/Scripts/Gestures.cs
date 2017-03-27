using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour {

	float handVelocity;
	Transform hand;
	Vector3 initialHandPos;
	Vector3 currentHandPos, lastHandPos;
	private Vector3 currentHandVelocity, lastHandVelocity;
	public float  currentHandVelocityMagnitude;
	Rigidbody rb;
	[Range (0.01f,0.1f)]
	public float handBoomThreshold;

	void Start () {
		hand = ControllerManager.Instance.primaryHand.transform;
		lastHandPos = hand.position;
		lastHandVelocity = Vector3.zero;
		//rb = hand.GetComponent<Rigidbody>();
		//print(rb);

	}

	void Update () {
		currentHandPos = hand.position;
		currentHandVelocity = currentHandPos - lastHandPos;
		currentHandVelocityMagnitude = currentHandVelocity.magnitude;
		float velocityChange = Vector3.Distance(lastHandVelocity, currentHandVelocity);
		//print(velocityChange);
		if (velocityChange > handBoomThreshold) {
			EventManager.TriggerEvent("HandBoom");
		}
		lastHandPos = currentHandPos;
		lastHandVelocity = currentHandVelocity;
	}
}
