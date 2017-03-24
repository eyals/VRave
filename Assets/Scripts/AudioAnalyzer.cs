using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyzer : MonoBehaviour {

	AudioSource audioSource;
	private float[] spectrum; //Audio Source data
	public FFTWindow FffWindowType = FFTWindow.BlackmanHarris;
	public int sampleRate = 1024;
	//public int spectromSampleRangeLow = 920;
	//public int spectrumSampleRangeHigh = 1000;
	private float volumeSum;
	private LightsManager lightsManager;
	[Range(0, 1)]
	public float spectrumMin = 0.1f;
	[Range(0, 1)]
	public float spectrumMax = 0.5f;


	void Start () {
		spectrum = new float[sampleRate];
		audioSource = GetComponent<AudioSource>();
		lightsManager = GameObject.Find("Lights").GetComponent<LightsManager>();
		//lightsManager.initLights();
	}

	void Update () {
		GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FffWindowType);
		int spectrumStart = Mathf.RoundToInt(spectrum.Length * spectrumMin);
		int spectrumEnd = Mathf.RoundToInt(spectrum.Length * spectrumMax);
		float[] trimmedSpectrum = new float[spectrumEnd-spectrumStart];
		System.Array.Copy(spectrum, spectrumStart, trimmedSpectrum, 0, spectrumEnd - spectrumStart);
		lightsManager.updateLights(spectrum);
		
		/*
		volumeSum = 0f;
		for (int i = spectromSampleRangeLow; i <= spectrumSampleRangeHigh; i++) {
			volumeSum += spectrum[i];
		}
		volumeSum = Mathf.Max(volumeSum, noiseThreshold);
		*/
	}
}
