using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour {

	public ParticleSystem pSides, pAbove;
	public List<ParticleCollisionEvent> collisionEvents;

	void Start() {
		ParticleSystem pSides = transform.Find("Sides").gameObject.GetComponent<ParticleSystem>();
		ParticleSystem pAbove = transform.Find("Above").gameObject.GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
	}

	void OnParticleCollision(GameObject other) {
		int numCollisionEvents = pAbove.GetCollisionEvents(other, collisionEvents);

		Rigidbody rb = other.GetComponent<Rigidbody>();
		int i = 0;

		while (i < numCollisionEvents) {
			if (rb) {
				Vector3 pos = collisionEvents[i].intersection;
				Vector3 force = collisionEvents[i].velocity * 10;
				rb.AddForce(force);
			}
			i++;
		}
	}
}
