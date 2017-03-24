using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {
	public AnalyzerSettings settings;
	private float lastVolume;
	private float targetIntensity;
	private float noiseThreshold = 10;
	private float almostZero = 0.1f;
	public float lightSizeFacor = 0.2f;
	public float lightIntensityFacor = 0.07f;
	private Transform bulb;
	private Material bulbMaterial;
	private float intensity, lastIntensity;
	public float intensityChangeSpeed = 0.1f;
	private Color lightColor;

	public void setIntensity(float level) {
		if (level<noiseThreshold) {
			level = 0;
		}
		targetIntensity = level;
	}

	private void Start() {
		bulb = transform.Find("Bulb");
		bulbMaterial = bulb.GetComponent<Renderer>().material;
		Vector3 pos = transform.localPosition;
		//pos.y += Random.Range(-3f, 5f);
		transform.localPosition = pos;
		lightColor = new Color(Random.Range(0.8f, 1), Random.Range(0.1f, 0.1f), Random.Range(0.3f, 0.6f), 1.0f);


	}

	private void Update() {
		intensity = Mathf.Lerp(lastIntensity, targetIntensity, intensityChangeSpeed);
		if (intensity < almostZero) intensity = 0;
		bulbMaterial.SetColor("_EmissionColor", lightColor * intensity * lightIntensityFacor);
		float bulbScale = intensity * lightSizeFacor;
		bulb.localScale = new Vector3(bulbScale, bulbScale, bulbScale);
		lastIntensity = intensity;

	}
}
