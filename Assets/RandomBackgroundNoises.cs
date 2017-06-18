using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackgroundNoises : MonoBehaviour {

    public enum BackgroundNoiseMode { Outside, Mine};
    public BackgroundNoiseMode mode = BackgroundNoiseMode.Outside;

    public AudioSource BirdSounds;
    public AudioSource CricketSounds;
    public AudioSource AmbientForestNoise;
    public AudioSource MineDrip;
    public float BirdFrequency;
    public float CricketFrequency;

    private float nextBirdSound;
    private float nextCricketSound;
    

	// Use this for initialization
	void Start () {
        nextBirdSound = Time.time + Random.Range(1.0f, BirdFrequency);
        nextCricketSound = Time.time +  Random.Range(1.0f, CricketFrequency);
        AmbientForestNoise.Play();
        AmbientForestNoise.loop = true;
    }

    // Update is called once per frame
    void Update () {
        if (mode == BackgroundNoiseMode.Outside)
        {
            if (nextBirdSound < Time.time)
            {
                BirdSounds.Play();
                nextBirdSound = Time.time + Random.Range(1.0f, BirdFrequency);

            }

            if (nextCricketSound < Time.time)
            {
                CricketSounds.Play();
                nextCricketSound = Time.time + Random.Range(1.0f, CricketFrequency);

            }
        } 
    }

    public void SwitchMode(BackgroundNoiseMode newMode)
    {
        mode = newMode;
        if (mode == BackgroundNoiseMode.Outside)
        {
            MineDrip.Stop();
            AmbientForestNoise.Play();
        } else if (mode == BackgroundNoiseMode.Mine)
        {
            MineDrip.Play();
            AmbientForestNoise.Stop();
        }
    }

}
