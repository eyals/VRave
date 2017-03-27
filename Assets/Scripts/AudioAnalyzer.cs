using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyzer : Singleton<AudioAnalyzer> {

	public FFTWindow FffWindowType = FFTWindow.BlackmanHarris;
	
	// Based on: http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
	//string mp3StreamUrl = "http://103.195.103.98/wp-includes/inc/stream.php?id=238041960&t=srvsc";
	//string mp3StreamUrl = "https://ia600309.us.archive.org/28/items/Mp3Songs_175/actionreplayy04www.songs.pk.mp3";
	public int fullSpectrumSize = 1024;  // array size
	public int SpectrumSize = 20; //sets the size of trimmed spectrum
	public float refValue = 0.1f; // RMS value for 0 dB
	public float threshold = 0.02f;      // minimum amplitude to extract pitch

	public float volAvg;    // sound level - Average
	public float lastVolAvg = 0;	
	public float volRms;	// sound level - RMS
	public float volDb;		// sound level - dB
	public float volMax;	// sound level - max sample

	public float[] fullSpectrum; // All the sampled audio
	public float[] spectrum; // The part of the audio in effect
	public float[] spectrumSqr; // The part of the audio in effect

	private AudioSource audio;
	private float timeSinceLastBoom = 0;
	private float minTimeBetweenBoom = 0.2f;

	void Start() {
		audio = GetComponent<AudioSource>();
		/*
		WWW www = new WWW(mp3StreamUrl);  // start a download of the given URL
		AudioClip clip = www.GetAudioClip(false, true); // 2D, streaming
		audio.clip = clip;
		audio.Play();
		*/
		fullSpectrum = new float[fullSpectrumSize];
	}

	public void AnalyzeSound() {

		//audio.GetOutputData(fullSpectrum, 0); // fill array with samples
		AudioListener.GetSpectrumData(fullSpectrum, 0, FFTWindow.Hamming);
		spectrum = new float[SpectrumSize];
		spectrumSqr = new float[SpectrumSize];
		System.Array.Copy(fullSpectrum, spectrum, SpectrumSize);
		float sum = 0;
		float supPow = 0;
		volMax = 0;
		for (int i = 0; i < SpectrumSize; i++) {
			//spectrumSqr[i] = Mathf.Sqrt(spectrum[i]);
			sum += spectrum[i];
			supPow += spectrum[i] * spectrum[i]; // sum squared samples
			if (spectrum[i] > volMax && spectrum[i] > threshold) {
				volMax = spectrum[i];
			}
		}
		volAvg = sum / SpectrumSize;
		volRms = Mathf.Sqrt(supPow/ SpectrumSize); // rms = square root of pow average 
		volDb = 20 * Mathf.Log10(volRms / refValue); // calculate dB

		triggerBoomEvents();

	}

	float volMultiply;
	float volAdd;
	void triggerBoomEvents() {
		
		timeSinceLastBoom += Time.deltaTime;
		if (timeSinceLastBoom < minTimeBetweenBoom) return;
		timeSinceLastBoom = 0;

		if (lastVolAvg > 0) {
			volMultiply = volMax / lastVolAvg;
			volAdd = volMax - lastVolAvg;
		}
		lastVolAvg = volAvg;
		//print(volAdd);


		if (volAdd > 0.25f) {
			print("h " + volAdd);
			EventManager.TriggerEvent("BoomHigh");
		}
		else if (volAdd > 0.2f) {
			print("m " + volAdd);
			EventManager.TriggerEvent("BoomMed");
		}
		else if (volAdd > 0.15f) {
			print("l " + volAdd);
			EventManager.TriggerEvent("BoomLow");
		}
	}
	void Update() {
		
		if (Input.GetKeyDown("p")) {
			if (audio.isPlaying) {
				audio.Pause();
			}else {
				audio.Play();
			}
			
		}
		AnalyzeSound();
	}
}
