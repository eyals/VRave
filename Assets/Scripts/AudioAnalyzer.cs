using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyzer : Singleton<AudioAnalyzer> {

	AudioSource audioSource;
	private float[] fullSpectrum; //Audio Source data
	public FFTWindow FffWindowType = FFTWindow.BlackmanHarris;
	//public int sampleRate;
	//public int spectromSampleRangeLow = 920;
	//public int spectrumSampleRangeHigh = 1000;
	//private float volumeSum;
	[Range(0, 1)]
	public float spectrumMin = 0.1f;
	[Range(0, 1)]
	public float spectrumMax = 0.5f;


	// Based on: http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
	//string mp3StreamUrl = "http://103.195.103.98/wp-includes/inc/stream.php?id=238041960&t=srvsc";
	//string mp3StreamUrl = "https://ia600309.us.archive.org/28/items/Mp3Songs_175/actionreplayy04www.songs.pk.mp3";
	public int spectrumSize = 1024;  // array size
	public int spectrumTrimSize = 20; //sets the size of trimmedSpectrum
	public float refValue = 0.1f; // RMS value for 0 dB
	public float threshold = 0.02f;      // minimum amplitude to extract pitch
	public float rmsValue;   // sound level - RMS
	public float dbValue;    // sound level - dB
	public float pitchValue; // sound pitch - Hz

	public float[] samples; // audio samples
	public float[] spectrum; // audio spectrum
	public float[] trimmedSpectrum; //part of the full spectrum
	private float fSample;
	private AudioSource audio;
 
	void Start() {
		audio = GetComponent<AudioSource>();
		/*
		WWW www = new WWW(mp3StreamUrl);  // start a download of the given URL
		AudioClip clip = www.GetAudioClip(false, true); // 2D, streaming
		audio.clip = clip;
		audio.Play();
		*/
		samples = new float[spectrumSize];
		spectrum = new float[spectrumSize];
		fSample = AudioSettings.outputSampleRate;
		//trimSize = Mathf.RoundToInt(Mathf.Sqrt(qSamples)/2);
	}
	public float maxV;
	public int maxN;
	void AnalyzeSound() {
		audio.GetOutputData(samples, 0); // fill array with samples
		float sum = 0;
		for (int i = 0; i < spectrumSize; i++) {
			sum += samples[i] * samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum / spectrumSize); // rms = square root of average
		dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
		if (dbValue < -160) dbValue = -160; // clamp it to -160dB min

		// get sound spectrum
		audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		trimmedSpectrum = new float[spectrumTrimSize];
		System.Array.Copy(spectrum, trimmedSpectrum, spectrumTrimSize);
	
		maxV = 0; //volume of laudest sample
		maxN = 0; // maxN is the index of max
		for (int i = 0; i < spectrumSize; i++) { // find max 
			if (spectrum[i] > maxV && spectrum[i] > threshold) {
				maxV = spectrum[i];
				maxN = i; 
			}
		}
		float freqN = maxN; // pass the index of the loudest sample to a float variable
		// interpolate index using neighbours
		if (maxN > 0 && maxN < spectrumSize - 1) { 
			float dL = spectrum[maxN - 1] / spectrum[maxN];
			float dR = spectrum[maxN + 1] / spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		pitchValue = freqN * (fSample / 2) / spectrumSize; // convert index to frequency
	}

	public float[] getTrimmedSpectrum(float min, float max) {
		int rangeMin = Mathf.RoundToInt(min * spectrumSize);
		int rangeMax = Mathf.RoundToInt(max * spectrumSize);
		int rangeSize = rangeMax - rangeMin;
		float[] s = new float[rangeSize];
		System.Array.Copy(spectrum, rangeMin, s, 0, rangeSize);
		return s;

	}
	public float[] getTrimmedSpectrum(float size) {
		int rangeSize = Mathf.RoundToInt(size * spectrumSize);
		float[] s = new float[rangeSize];
		System.Array.Copy(spectrum, s, rangeSize);
		return s;
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
