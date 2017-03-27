using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBeatballs : MonoBehaviour {

	public GameObject lightObject;
	[Range(1, 1000)]
	public int lightsCount;
	[Range(0, 10)]
	public float lightDistanceMin, lightDistanceMax;
	[Range(0, 10)]
	public float lightHeightMin, lightHeightMax;
	//private List<GameObject> lightObjects;
	private SoundLight floorLight;
	public Color colorRangeMin, colorRangeMax;
	private Transform lights;
	[Range(0.005f,0.2f)]
	public float spectrumRangeMax;
	private float lastSpectrumRangeMax;

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

		
		colorRangeMin = Random.ColorHSV();
		colorRangeMax = Random.ColorHSV();
		float minHue, minColorS, minColorV;
		float maxHue, maxColorS, maxColorV;
		Color.RGBToHSV(colorRangeMin, out minHue, out minColorS, out minColorV);
		Color.RGBToHSV(colorRangeMax, out maxHue, out maxColorS, out maxColorV);
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


		floorLight = transform.Find("Floor").GetComponent<SoundLight>();
		floorLight.lightColor = randomColorBetweetHues(minHue, maxHue);
		floorLight.reset();

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

		if (spectrumRangeMax!=lastSpectrumRangeMax) {
			Randomize();
			lastSpectrumRangeMax = spectrumRangeMax;
		}

		float[] spectrum = AudioAnalyzer.Instance.getTrimmedSpectrum(spectrumRangeMax);
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
		floorLight.setLevel(floorIntensity);
	}
}
