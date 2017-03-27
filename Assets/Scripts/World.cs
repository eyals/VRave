using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class World : Singleton<World> {

	public static Vector3 viewerPosition;
	public static GameObject player;

	void Awake() {
		player = GameObject.FindWithTag("Player");
	}

	void Start () {
		
		
	}
	void Update() {
		//viewerPosition = GameObject.Find("CenterEyeAnchor").transform.position;
	}



}
