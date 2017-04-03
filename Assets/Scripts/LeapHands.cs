using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapHands : Singleton<LeapHands> {

	public bool RightHandFound, LeftHandFound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		print(GameObject.Find("LeapHandL"));
	}
}
