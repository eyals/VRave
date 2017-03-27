using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour {

	float handVelocity;
	Transform hand;
	Vector3 initialHandPos;
	Vector3 currentHandPos, lastHandPos;
	Vector3 currentHandVelocity, lastHandVelocity;
	Rigidbody rb;

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
		float velocityChange = Vector3.Distance(lastHandVelocity, currentHandVelocity);
		//print(velocityChange);
		if (velocityChange > 0.05f) {
			EventManager.TriggerEvent("HandBoom");
		}
		lastHandPos = currentHandPos;
		lastHandVelocity = currentHandVelocity;
	}
}
