using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour {

	public GameObject lightObject;
	[Range(1, 1000)]
	public int lightsCount;
	[Range (1,1000)]
	public float ringAngle;
	public float lightDistanceMin;
	public float lightDistanceMax;
	public float lightHeightMin;
	public float lightHeightMax;
	private List<GameObject> lights;

	private void Start() {
		for (var i=0; i<lightsCount; i++) {
			GameObject light = Instantiate(lightObject, transform);
			light.name = "light" + i;
			light.GetComponent<SoundLight>().index = i;
			float lightAngle = ringAngle / lightsCount * (i - lightsCount / 2);
			//float lightAngle = Random.Range(0, 360); 
			light.transform.Rotate(Vector3.up, lightAngle);
			float lightBulbDistance = Random.Range(lightDistanceMin, lightDistanceMax);
			float lightBulbHeight = Random.Range(lightHeightMin, lightHeightMax);
			light.GetComponent<SoundLight>().setPosition(lightBulbDistance, lightBulbHeight);
		}
	

	}

	private void Update() {
		float[] spectrum = AudioAnalyzer.Instance.trimmedSpectrum;
		if (spectrum.Length < 1) return;
		float[] lightLevels = new float[lightsCount];
		int lightSpectrumRange = Mathf.FloorToInt(spectrum.Length / lightsCount); //some remainders from the upper edge of the spectrom get lost here
		for (var i = 0; i < lightsCount; i++) {
			for (var s = 0; s < lightSpectrumRange; s++) {
				lightLevels[i] += spectrum[i* lightSpectrumRange + s]/ lightSpectrumRange;//becomes the avg of that spectrum range
			}

		}

		for (var i = 0; i < lightsCount; i++) {
			int spectrumItem;
			if (lightsCount< spectrum.Length) {
				spectrumItem = Mathf.RoundToInt(spectrum.Length / lightsCount * i);
			}else {
				spectrumItem = i % spectrum.Length;
			}
			transform.Find("light" + i).GetComponent<SoundLight>().setIntensity(spectrum[spectrumItem]);
			transform.Find("light" + i).GetComponent<SoundLight>().setIntensity(lightLevels[i]);
		}
		//float floorIntensity = AudioAnalyzer.Instance.maxV;
		float floorIntensity = AudioAnalyzer.Instance.rmsValue/3;
		//if (floorIntensity < 0.2f) floorIntensity = 0.02f;
		if (floorIntensity < 0.7f) floorIntensity *= 0.5f;
		if (floorIntensity > 0.7f) floorIntensity *= 3;
		GameObject.Find("Floor").GetComponent<SoundLight>().setIntensity(floorIntensity);
	}
}
