using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyzer : Singleton<AudioAnalyzer> {

	AudioSource audioSource;
	private float[] fullSpectrum; //Audio Source data
	public FFTWindow FffWindowType = FFTWindow.BlackmanHarris;
	public int sampleRate;
	//public int spectromSampleRangeLow = 920;
	//public int spectrumSampleRangeHigh = 1000;
	//private float volumeSum;
	private LightsManager lightsManager;
	[Range(0, 1)]
	public float spectrumMin = 0.1f;
	[Range(0, 1)]
	public float spectrumMax = 0.5f;

	/*
	void Start () {
		audioSource = GetComponent<AudioSource>();
		lightsManager = GameObject.Find("Lights").GetComponent<LightsManager>();
		//lightsManager.initLights();
	}

	public float[] currentSpectrum () {
		fullSpectrum = new float[sampleRate];
		GetComponent<AudioSource>().GetSpectrumData(fullSpectrum, 0, FffWindowType);
		int spectrumStart = Mathf.RoundToInt(fullSpectrum.Length * spectrumMin);
		int spectrumEnd = Mathf.RoundToInt(fullSpectrum.Length * spectrumMax);
		float[] trimmedSpectrum = new float[spectrumEnd-spectrumStart];
		System.Array.Copy(fullSpectrum, spectrumStart, trimmedSpectrum, 0, spectrumEnd - spectrumStart);
		//lightsManager.updateLights(fullSpectrum);
		return fullSpectrum;
		
		
		//volumeSum = 0f;
		//for (int i = spectromSampleRangeLow; i <= spectrumSampleRangeHigh; i++) {
			//volumeSum += spectrum[i];
		//}
		//volumeSum = Mathf.Max(volumeSum, noiseThreshold);
	}
		*/

	// Source: http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
	public int qSamples = 1024;  // array size
	public float refValue = 0.1f; // RMS value for 0 dB
	public float threshold = 0.02f;      // minimum amplitude to extract pitch
	public float rmsValue;   // sound level - RMS
	public float dbValue;    // sound level - dB
	public float pitchValue; // sound pitch - Hz

	public float[] samples; // audio samples
	public float[] spectrum; // audio spectrum
	public float[] trimmedSpectrum; //part of the full spectrum
	private int trimSize;
	private float fSample;
	private AudioSource audio;
 
	void Start() {
		samples = new float[qSamples];
		spectrum = new float[qSamples];
		fSample = AudioSettings.outputSampleRate;
		audio = GetComponent<AudioSource>();
		trimSize = Mathf.RoundToInt(Mathf.Sqrt(qSamples)/2);
	}
	public float maxV;
	public int maxN;
	void AnalyzeSound() {
		audio.GetOutputData(samples, 0); // fill array with samples
		float sum = 0;
		for (int i = 0; i < qSamples; i++) {
			sum += samples[i] * samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
		dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
		if (dbValue < -160) dbValue = -160; // clamp it to -160dB min

		// get sound spectrum
		audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		trimmedSpectrum = new float[trimSize];
		System.Array.Copy(spectrum, trimmedSpectrum, trimSize);
	
		maxV = 0; //volume of laudest sample
		maxN = 0; // maxN is the index of max
		for (int i = 0; i < qSamples; i++) { // find max 
			if (spectrum[i] > maxV && spectrum[i] > threshold) {
				maxV = spectrum[i];
				maxN = i; 
			}
		}
		float freqN = maxN; // pass the index of the loudest sample to a float variable
		// interpolate index using neighbours
		if (maxN > 0 && maxN < qSamples - 1) { 
			float dL = spectrum[maxN - 1] / spectrum[maxN];
			float dR = spectrum[maxN + 1] / spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		pitchValue = freqN * (fSample / 2) / qSamples; // convert index to frequency
	}

	public GUIText display; // drag a GUIText here to show results
 
 void Update() {
		if (Input.GetKeyDown("p")) {
			audio.Play();
		}
		AnalyzeSound();
		if (display) {
			display.text = "RMS: " + rmsValue.ToString("F2") +
			" (" + dbValue.ToString("F1") + " dB)\n" +
			"Pitch: " + pitchValue.ToString("F0") + " Hz";
		}
	}
}
