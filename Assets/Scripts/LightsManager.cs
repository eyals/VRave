using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour {

	public int lightsCount = 1;
	public GameObject lightObject;
	private List<GameObject> lights;
	public float ringAngle = 180f;
	public float lightDistanceMin = 4f;
	public float lightDistanceMax = 4f;
	public float lightHeightMin = 1f;
	public float lightHeightMax = 1f;

	private void Start() {
		for (var i=0; i<lightsCount; i++) {
			GameObject light = Instantiate(lightObject, transform);
			light.name = "light" + i;
			light.GetComponent<SoundLight>().index = i;
			float lightAngle = ringAngle / lightsCount * (i - lightsCount / 2);
			light.transform.Rotate(Vector3.up, lightAngle);
			float lightBulbDistance = Random.Range(lightDistanceMin, lightDistanceMax);
			float lightBulbHeight = Random.Range(lightHeightMin, lightHeightMax);
			light.GetComponent<SoundLight>().setPosition(lightBulbDistance, lightBulbHeight);
		}
	}

	public void updateLights(float[] spectrum) {
		float[] lightLevels = new float[lightsCount];
		int spectrumSize = spectrum.Length;
		int lightSpectrumRange = Mathf.RoundToInt(spectrumSize / lightsCount);
		for (var s=0; s<spectrum.Length; s++) {
			int designatedLight = Mathf.FloorToInt(s / lightSpectrumRange);
			if (designatedLight < lightsCount) {
				lightLevels[designatedLight] += spectrum[s];
			}
		}
		for (var i = 0; i < lightsCount; i++) {
			//int spectrumItem = Mathf.RoundToInt(spectrum.Length / lightsCount * i);
			//transform.Find("light" + i).GetComponent<SoundLight>().setIntensity(spectrum[spectrumItem]);
			transform.Find("light" + i).GetComponent<SoundLight>().setIntensity(lightLevels[i]);
		}
	}
}
