using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBeatballs : MonoBehaviour {

	public GameObject lightObject;
	[Range(1, 1000)]
	public int lightsCount;
	[Range(1, 1000)]
	public float ringAngle;
	public float lightDistanceMin;
	public float lightDistanceMax;
	public float lightHeightMin;
	public float lightHeightMax;
	//private List<GameObject> lightObjects;
	private SoundLight floor;
	public Color colorScaleMin;
	public Color colorScaleMax;
	private Transform lights;

	private void Start() {
		lights = new GameObject("lights").transform;
		for (var i = 0; i < lightsCount; i++) {
			GameObject light = Instantiate(lightObject, transform);
			light.name = "light" + i;
			light.transform.parent = lights;
		}
		Randomize();
	}

	private void Randomize() {

		
		colorScaleMin = Random.ColorHSV();
		colorScaleMax = Random.ColorHSV();
		float minHue, minColorS, minColorV;
		float maxHue, maxColorS, maxColorV;
		Color.RGBToHSV(colorScaleMin, out minHue, out minColorS, out minColorV);
		Color.RGBToHSV(colorScaleMax, out maxHue, out maxColorS, out maxColorV);
		print(minHue + "," + maxHue);

		for (var i = 0; i < lightsCount; i++) {
			SoundLight sl = lights.Find("light" + i).GetComponent<SoundLight>();
			sl.transform.localRotation = Quaternion.identity;
			sl.transform.Rotate(Vector3.up, Random.Range(0, 360));
			sl.index = i;
			sl.distanceFromViewer = Random.Range(lightDistanceMin, lightDistanceMax);
			sl.height = Random.Range(lightHeightMin, lightHeightMax);
			float frequencyIndex = Mathf.FloorToInt(i / AudioAnalyzer.Instance.spectrumTrimSize);
			//sl.frequencyRatio = 1 / frequencyIndex;//translates frequencyIndex to 0..1 
			sl.lightColor = randomColorBetweetHues(minHue, maxHue);
			//sl.lightColor = colorLerpBetweetHues(minHue, maxHue, sl.frequencyRatio); //not working as expected
			sl.reset();
		}


		floor = transform.Find("Floor").GetComponent<SoundLight>();
		floor.lightColor = randomColorBetweetHues(minHue, maxHue);
		floor.reset();

		ParticleSystem particlesSides = transform.Find("Particles").Find("Sides").gameObject.GetComponent<ParticleSystem>();
		ParticleSystem particlesAbove = transform.Find("Particles").Find("Above").gameObject.GetComponent<ParticleSystem>();
		particlesSides.startColor = randomColorBetweetHues(minHue, maxHue);
		particlesAbove.startColor = randomColorBetweetHues(minHue, maxHue);
	}

	private Color randomColorBetweetHues(float min, float max) {
		float h;
		if (min > max) {
			h = (Random.Range(min - max, 1) + max) % 1;
		}
		else {
			h = Random.Range(min, max);
		}
		return Color.HSVToRGB(h, 1, 1);
	}

	private Color colorLerpBetweetHues(float min, float max, float t) {
		float h;
		if (min > max) {
			h = (Mathf.Lerp(min - max, 1, t) + max) % 1;
		}
		else {
			h = Mathf.Lerp(min, max, t);
		}
		return Color.HSVToRGB(h, 1, 1);
	}


	private void Update() {

		if (Input.GetKeyDown(KeyCode.R)) Randomize();

		float[] spectrum = AudioAnalyzer.Instance.getTrimmedSpectrum(0.02f);
		if (spectrum.Length < 1) return;
		float[] lightLevels = new float[lightsCount];
		int lightSpectrumRange = Mathf.FloorToInt(spectrum.Length / lightsCount); //some remainders from the upper edge of the spectrom get lost here
		for (var i = 0; i < lightsCount; i++) {
			for (var s = 0; s < lightSpectrumRange; s++) {
				lightLevels[i] += spectrum[i * lightSpectrumRange + s] / lightSpectrumRange;//becomes the avg of that spectrum range
			}

		}

		for (var i = 0; i < lightsCount; i++) {
			int spectrumItem;
			if (lightsCount < spectrum.Length) {
				spectrumItem = Mathf.RoundToInt(spectrum.Length / lightsCount * i);
			}
			else {
				spectrumItem = i % spectrum.Length;
			}
			lights.Find("light" + i).GetComponent<SoundLight>().setLevel(spectrum[spectrumItem]);
			//transform.Find("light" + i).GetComponent<SoundLight>().setLevel(lightLevels[i]);
		}
		//float floorIntensity = AudioAnalyzer.Instance.maxV;
		float floorIntensity = AudioAnalyzer.Instance.maxV;
		//if (floorIntensity < 0.2f) floorIntensity = 0.02f;
		//floorIntensity *= (floorIntensity > 0.8f) ? 3 : 0.02f;
		floor.setLevel(floorIntensity);
	}
}
