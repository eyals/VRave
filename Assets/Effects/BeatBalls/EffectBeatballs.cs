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
	[Range(1, 1024)]
	public int spectrumRangeMax;
	private float lastSpectrumRangeMax;
	private Transform handBoomSource;
	//private ParticleSystem handBoomParticles;
	private float minHue, minColorS, minColorV;
	private float maxHue, maxColorS, maxColorV;
	public GameObject handObj;
	private HandGesture handGestureR, handGestureL;

	private void Start() {
		lights = new GameObject("lights").transform;
		lights.parent = transform;
		for (var i = 0; i < lightsCount; i++) {
			GameObject light = Instantiate(lightObject, transform);
			light.name = "light" + i;
			light.transform.parent = lights;
		}
		//handBoomSource = transform.Find("Boom");
		//handBoomParticles = handBoomSource.gameObject.GetComponent<ParticleSystem>();

		AudioAnalyzer.Instance.SpectrumSize = spectrumRangeMax;

		GameObject handR = Instantiate(handObj, ControllerManager.Instance.primaryHand.transform);
		GameObject handL = Instantiate(handObj, ControllerManager.Instance.secondaryHand.transform);
		handR.transform.localPosition = Vector3.zero;
		handL.transform.localPosition = Vector3.zero;
		handGestureR = handR.GetComponent<HandGesture>();
		handGestureL = handL.GetComponent<HandGesture>();

		EventManager.StartListening("primaryTopButtonPressed", randomize);
		//EventManager.StartListening("HandBoom", handBoom);
		//EventManager.StartListening("BoomLow", handBoom);
		EventManager.StartListening("BoomMed", randomize);
		randomize();
	}

	private void randomize() {

		
		colorRangeMin = Random.ColorHSV();
		colorRangeMax = Random.ColorHSV();
		Color.RGBToHSV(colorRangeMin, out minHue, out minColorS, out minColorV);
		Color.RGBToHSV(colorRangeMax, out maxHue, out maxColorS, out maxColorV);

		for (var i = 0; i < lightsCount; i++) {
			SoundLight sl = lights.Find("light" + i).GetComponent<SoundLight>();
			float frequencyIndex = Mathf.FloorToInt(i / AudioAnalyzer.Instance.SpectrumSize);
			float frequencyRatio = frequencyIndex / (lightsCount/AudioAnalyzer.Instance.SpectrumSize);//translates frequencyIndex to 0..1 
			//print("~"+ frequencyRatio);
			sl.transform.localRotation = Quaternion.identity;
			sl.transform.Rotate(Vector3.up, Random.Range(0,360));
			sl.index = i;
			//sl.distanceFromViewer = Mathf.Lerp(lightDistanceMin, lightDistanceMax, 1-frequencyRatio);
			sl.distanceFromViewer =Random.Range(lightDistanceMin, lightDistanceMax);
			//sl.height = Mathf.Lerp(lightHeightMin, lightHeightMax, frequencyRatio);
			sl.height = Random.Range(lightHeightMin, lightHeightMax);
			sl.lightColor = randomColorBetweetHues(minHue, maxHue);
			sl.lightColor = colorLerpBetweetHues(minHue, maxHue, frequencyRatio); //not working as expected
			sl.reset();
		}

		handGestureR.color = randomColorBetweetHues(minHue, maxHue);
		handGestureL.color = randomColorBetweetHues(minHue, maxHue);

		randomizeFloor();

		ParticleSystem particlesSides = transform.Find("Particles").Find("Sides").gameObject.GetComponent<ParticleSystem>();
		ParticleSystem particlesAbove = transform.Find("Particles").Find("Above").gameObject.GetComponent<ParticleSystem>();
		particlesSides.startColor = randomColorBetweetHues(minHue, maxHue);
		particlesAbove.startColor = randomColorBetweetHues(minHue, maxHue);
	}

	private void randomizeFloor() {
		floorLight = transform.Find("Floor").GetComponent<SoundLight>();
		floorLight.lightColor = randomColorBetweetHues(minHue, maxHue);
		floorLight.reset();

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

		if (Input.GetKeyDown(KeyCode.R)) randomize();

		if (spectrumRangeMax!=lastSpectrumRangeMax) {
			randomize();
			lastSpectrumRangeMax = spectrumRangeMax;
		}

		float[] spectrum = AudioAnalyzer.Instance.spectrum;
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
		float floorIntensity = AudioAnalyzer.Instance.volAvg * 5;
		if (floorIntensity < 0.1f) floorIntensity = 0;
		floorLight.setLevel(floorIntensity);


	}

}
