using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackgroundNoises : MonoBehaviour {

    public AudioSource BirdSounds;
    public AudioSource CricketSounds;
    public float BirdFrequency;
    public float CricketFrequency;

    private float nextBirdSound;
    private float nextCricketSound;
    

	// Use this for initialization
	void Start () {
        nextBirdSound = Time.time + Random.Range(1.0f, BirdFrequency);
        nextCricketSound = Time.time +  Random.Range(1.0f, CricketFrequency);
    }

    // Update is called once per frame
    void Update () {
	    if (nextBirdSound < Time.time)
        {
            BirdSounds.Play();
            nextBirdSound = Time.time +  Random.Range(1.0f, BirdFrequency);

        }

        if (nextCricketSound < Time.time)
        {
            CricketSounds.Play();
            nextCricketSound = Time.time + Random.Range(1.0f, CricketFrequency);

        }
    }
}
