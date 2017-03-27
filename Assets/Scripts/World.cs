using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class World : Singleton<World> {

	public static Vector3 viewerPosition;
	public static Vector3 viewerRotation;
	public static Transform player;
	public static Transform camera;

	void Awake() {
		player = GameObject.FindWithTag("Player").transform;
		camera = GameObject.Find("CenterEyeAnchor").transform;
	}

	void Start () {
		
		
	}
	void Update() {
		
	}



}
