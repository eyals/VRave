using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLight : MonoBehaviour {

	public int index;
	private Transform bulb;
	//public float lightScale = 10000000f;
	private float lastLevel = 0;
	public float lightScaleSpeed = 0.1f;
	public float maxScale = 1f;
	public Color lightColor;
	private Material bulbMaterial;
	public float lightIntensityFacor = 20f;
	private float lightScaleFacor = 0.8f;
	public float audioMinThreshold = 0.05f;
	public bool enableScale = true;
	public bool enableMove = true;
	public bool enableShake = true;
	public bool enableSparks = true;
	Transform pole;
	private Vector3 lightPos;
	public float maxHeightChange = 1f;
	public float retractSpeedFactor = 1;
	public float distanceFromViewer;
	public float height;
	public float frequencyRatio;// 0..1 - 0 is bass, 1 is trebble
	private ParticleSystem sparks;
	public float sparksThreshold = 0.3f;


	void Awake () {
		bulb = transform.Find("Bulb");
		bulbMaterial = bulb.GetComponent<Renderer>().material;
		pole = transform.Find("Pole");

	}

	public void init() {
		bulb.localPosition = new Vector3(0, height, distanceFromViewer);
		lightPos = bulb.localPosition;
		if (pole) {
			pole.localPosition = lightPos;
		}
		if (enableSparks) {
			sparks = bulb.transform.Find("Sparks").gameObject.GetComponent<ParticleSystem>();
		}
	}


	public void setLevel(float level) {
		if (level < audioMinThreshold) level = 0;
		float targetLevel;

		float retractSpeed = Mathf.Clamp(level,0.01f,1)* retractSpeedFactor;

		if (level>lastLevel) {
			targetLevel = level;
		}else {
			targetLevel = (lastLevel + level*retractSpeed) / (1+retractSpeed); //Mathf.Lerp(lastLevel, level, lightScaleSpeed);
		}
		//if (index == 3) print(level + "->" + targetLevel);

		bulbMaterial.SetColor("_EmissionColor", lightColor * targetLevel * lightIntensityFacor);

		if (enableScale) {
			//float bulbScale = lightScale * targetLevel * targetLevel;
			float bulbScale = targetLevel* lightScaleFacor;
			//bulbScale *= (1 - frequencyRatio);
			if (targetLevel > 0.2f) { bulbScale *= 4; }
			if (targetLevel > 0.4f) { bulbScale *= 8; }
			//if (targetLevel > 0.8f) { bulbScale *= 2; }
			if (targetLevel > 0.9f) { bulbScale *= 2; }
			
			bulb.localScale = new Vector3(bulbScale, bulbScale, bulbScale);
		}
		if (enableShake) {
			if (targetLevel>0.4f) {
				bulb.localScale *= Random.Range(1f, 1.3f);
				bulb.localScale *= Mathf.Sqrt(AudioAnalyzer.Instance.rmsValue);
				float shake = bulb.localScale.x * Random.Range(-0.05f, 0.05f);
				bulb.localPosition = new Vector3(lightPos.x+ shake, lightPos.y, lightPos.z + bulb.localScale.x * shake); ;
			}else {
				bulb.localPosition = lightPos;
			}
		}
		if (enableSparks) {
			if (!sparks) return;
			if (targetLevel> sparksThreshold) {
				sparks.Play();		
			}else {
				sparks.Stop();
			}
		}
		if (enableMove) {
			/*
				float heightLowerMultiplier = 10;
				float hightLower;
				if (level > lastLevel) {
					hightLower = level * heightLowerMultiplier;
				}else {
					hightLower = level;
				}
				*/
			float hightLower = bulb.localScale.y / 2;
			//bulb.localPosition = new Vector3(bulb.localPosition.x, myPos.y - hightLower, bulb.localPosition.z);
		}
		if (pole) {
			pole.GetComponent<Renderer>().material = bulbMaterial;
			float targetHeight = bulb.localPosition.y + bulb.localScale.y / 2 + pole.localScale.y / 2;
			pole.localPosition = new Vector3(pole.localPosition.x, targetHeight, pole.localPosition.z);
			float poleThickness = 0.1f * targetLevel* targetLevel;
			pole.localScale = new Vector3(poleThickness, pole.localScale.y, poleThickness);
		}
		lastLevel = targetLevel;
	}
}
