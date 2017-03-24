using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLight : MonoBehaviour {

	public int index;
	private Transform bulb;
	public float lightScale =1f;
	public float lastLevel = 0;
	public float lightScaleSpeed = 0.001f;
	public float maxScale = 10f;
	private Color lightColor;
	private Material bulbMaterial;
	private float lightIntensityFacor = 3f;
	private float audioMinThreshold = 0.05f;


	// Use this for initialization
	void Awake () {
		bulb = transform.Find("Bulb");
		bulbMaterial = bulb.GetComponent<Renderer>().material;
		lightColor = new Color(Random.Range(0.5f, 1), Random.Range(0.1f, 0.1f), Random.Range(0.3f, 0.6f), 1.0f);
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setPosition(float distance, float height) {
		bulb.localPosition = new Vector3(0, height, distance);
	}

	public void setIntensity(float level) {
		if (level < audioMinThreshold) level = 0;
		float targetLevel;
		if (level>lastLevel) {
			targetLevel = (lastLevel + level) / 2;
		}
		else {
			targetLevel = (lastLevel * 4 + level) / 5; //Mathf.Lerp(lastLevel, level, lightScaleSpeed);
		}
		if (index == 0) {
			targetLevel *= 0.5f;
		}
		else {
			//targetLevel *= (index*index/200);

		}

		float bulbScale = targetLevel * Time.deltaTime * 200;
		//bulbScale = Mathf.Min(maxScale, bulbScale);
		bulb.localScale = new Vector3(bulbScale, bulbScale, bulbScale);
		bulbMaterial.SetColor("_EmissionColor", lightColor* targetLevel*20);
		lastLevel = targetLevel;
	}
}
