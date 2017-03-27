using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLight : MonoBehaviour {

	public int index;
	private Transform bulb;
	private Renderer bulbRend;
	//public float lightScale = 10000000f;
	private float lastLevel = 0;
	public float lightScaleSpeed = 0.1f;
	public float maxScale = 1f;
	private float minLevel = 0.02f;
	public Color lightColor;
	private Material bulbMaterial;
	private float lightIntensityFacor = 5f;
	private float lightScaleFacor = 3f;
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
	private ParticleSystem sparks;
	private float sparksThreshold = 0.3f;
	private float shakeThreshold = 0.3f;
	public float hue;
	public float targetLevel;


	void Awake () {
		bulb = transform.Find("Bulb");
		bulbRend = bulb.GetComponent<Renderer>();
		bulbMaterial = bulbRend.material;
		pole = transform.Find("Pole");

	}

	public void reset() {
		//lightColor = Color.HSVToRGB(hue, 1, 1);
		bulb.localPosition = new Vector3(0, height, distanceFromViewer);
		lightPos = bulb.localPosition;
		if (pole) {
			pole.localPosition = lightPos;
		}
		if (enableSparks) {
			sparks = bulb.transform.Find("Sparks").gameObject.GetComponent<ParticleSystem>();
			//sparks.startColor = lightColor;
		}
	}


	public void setLevel(float level) {

		if (!bulbRend.isVisible) return;

		if (level < audioMinThreshold) level = minLevel;

		float retractSpeed = Mathf.Clamp(level,0.01f,1)* retractSpeedFactor;
		if (level>lastLevel) {
			targetLevel = level;
		}else {
			targetLevel = (lastLevel + level*retractSpeed) / (1+retractSpeed); //Mathf.Lerp(lastLevel, level, lightScaleSpeed);
		}
		lastLevel = targetLevel;


		bulbMaterial.SetColor("_EmissionColor", lightColor * targetLevel * lightIntensityFacor);

		if (enableScale) {
			//float bulbScale = lightScale * targetLevel * targetLevel;
			float bulbScale = targetLevel* lightScaleFacor;
			//bulbScale *= (1 - frequencyRatio);
			//bulbScale *= (1 - AudioAnalyzer.Instance.volAvg);
			if (targetLevel > 0.2f) { bulbScale *= 2; }
			//if (targetLevel > 0.4f) { bulbScale *= 8; }
			if (targetLevel > 0.6f) { bulbScale *= 2; }
			//if (targetLevel > 0.9f) { bulbScale *= 2; }

			bulb.localScale = new Vector3(bulbScale, bulbScale, bulbScale);
		}
		if (enableShake) {
			if (targetLevel> shakeThreshold) {
				//bulb.localScale *= Random.Range(1f, 1.3f);
				//bulb.localScale *= Mathf.Sqrt(AudioAnalyzer.Instance.rmsValue);
				float shakeSize = 0.1f;
				float shakeX = bulb.localScale.x * Random.Range(-shakeSize, shakeSize);
				float shakeZ = bulb.localScale.z * Random.Range(-shakeSize, shakeSize);
				bulb.localPosition = new Vector3(lightPos.x+shakeX, lightPos.y, lightPos.z+shakeZ); ;
			}else {
				bulb.localPosition = lightPos;
			}
		}
		if (enableSparks && sparks) {
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
			float poleThickness = 0.001f * distanceFromViewer;
			pole.localScale = new Vector3(poleThickness, pole.localScale.y, poleThickness);
		}

	}
}
