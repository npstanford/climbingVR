using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class WristTimerController : MonoBehaviour {

    public GameController gc;

    public bool Displayed;
    public Text WristDisplay;
    public TimeSpan CurrentTime;

	// Use this for initialization
	void Start () {
        //maybe need to pull the time and make it a timespan
	}
	
	// Update is called once per frame
	void Update () {
        CurrentTime = TimeSpan.FromSeconds(gc.TimeElapsed());
        WristDisplay.text = CurrentTime.ToString();
    }

}
