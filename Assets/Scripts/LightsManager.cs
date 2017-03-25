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
	private SoundLight floor;

	private void Start() {
		for (var i=0; i<lightsCount; i++) {

			GameObject light = Instantiate(lightObject, transform);
			light.name = "light" + i;
			//float lightAngle = ringAngle / lightsCount * (i - lightsCount / 2);
			float lightAngle = Random.Range(0, 360); 
			light.transform.Rotate(Vector3.up, lightAngle);

			SoundLight sl = light.GetComponent<SoundLight>();
			sl.index = i;
			sl.distanceFromViewer = Random.Range(lightDistanceMin, lightDistanceMax);
			sl.height = Random.Range(lightHeightMin, lightHeightMax);
			float frequencyIndex = Mathf.FloorToInt(i / AudioAnalyzer.Instance.spectrumTrimSize);
			sl.frequencyRatio = 1/frequencyIndex;//translates frequencyIndex to 0..1 // frequencyIndex / (lightsCount / AudioAnalyzer.Instance.spectrumTrimSize);
			sl.lightColor = new Color(sl.frequencyRatio, 0f, 1 - sl.frequencyRatio, 1);//red on high frequencies
			//sl.lightColor = new Color(Random.Range(0.1f, 1), Random.Range(0.1f, 0.1f), Random.Range(0.3f, 0.6f));
			sl.init();
		}
		floor = GameObject.Find("Floor").GetComponent<SoundLight>();
		floor.lightColor = new Color(0.3f,0.3f,1f, 1f);
		floor.init();

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
			transform.Find("light" + i).GetComponent<SoundLight>().setLevel(spectrum[spectrumItem]);
			//transform.Find("light" + i).GetComponent<SoundLight>().setLevel(lightLevels[i]);
		}
		//float floorIntensity = AudioAnalyzer.Instance.maxV;
		float floorIntensity = AudioAnalyzer.Instance.maxV;
		//if (floorIntensity < 0.2f) floorIntensity = 0.02f;
		//floorIntensity *= (floorIntensity > 0.8f) ? 3 : 0.02f;
		floor.setLevel(floorIntensity);
	}
}
