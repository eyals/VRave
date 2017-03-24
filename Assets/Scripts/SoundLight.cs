using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLight : MonoBehaviour {

	public int index;
	private Transform bulb;
	public float lightScale =8f;
	private float lastLevel = 0;
	public float lightScaleSpeed = 0.1f;
	public float maxScale = 1f;
	private Color lightColor;
	private Material bulbMaterial;
	private float lightIntensityFacor = 20f;
	private float audioMinThreshold = 0.05f;
	public bool enableScale = true;
	Transform pole;


	// Use this for initialization
	void Awake () {
		bulb = transform.Find("Bulb");
		bulbMaterial = bulb.GetComponent<Renderer>().material;
		lightColor = new Color(Random.Range(0.1f, 1), Random.Range(0.1f, 0.1f), Random.Range(0.3f, 0.6f), 1.0f);
		pole = transform.Find("Pole");
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setPosition(float distance, float height) {
		bulb.localPosition = new Vector3(0, height, distance);
		if (pole) {
			pole.localPosition = bulb.localPosition;
			pole.Translate(0, pole.localScale.y / 2, 0, Space.Self);
		}
	}

	public void setIntensity(float level) {
		//level = level/(index+1);
		if (level < audioMinThreshold) level = 0;
		float targetLevel;
		
		if (level>lastLevel) {
			targetLevel = level;
		}else {
			targetLevel = (lastLevel * 7 + level) / 8; //Mathf.Lerp(lastLevel, level, lightScaleSpeed);
		}

		if (index == 0) {
			//targetLevel *= 0.5f;//the most left one is too extreme
		}else {
			//targetLevel *= (index*index/200); // giving a chance to the small ones (?) to shine
		}

		bulbMaterial.SetColor("_EmissionColor", lightColor * targetLevel * lightIntensityFacor);

		if (enableScale) {
			float bulbScale = targetLevel;
			if (targetLevel > 0.2f) bulbScale *= 4;
			if (targetLevel > 0.4f) bulbScale *= 4;
			if (targetLevel > 0.7f) bulbScale *= 4;
			//bulbScale = Mathf.Min(maxScale, bulbScale); //limiting the size. Fail. Kills the beat.
			bulb.localScale = new Vector3(bulbScale, bulbScale, bulbScale);
		}
		if (pole) {
			pole.GetComponent<Renderer>().material = bulbMaterial;
		}
		lastLevel = targetLevel;
	}
}
