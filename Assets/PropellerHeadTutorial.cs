using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerHeadTutorial : MonoBehaviour {

    public SkyHookSpeaker Speaker;
    private bool PlayedTutorial = false;

	// Use this for initialization
	void Start () {

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !PlayedTutorial)
        {
            Speaker.LaunchAudio(SkyHookSpeaker.SpeakerPrograms.PropellerHead);
            PlayedTutorial = true;
        }
    }
}
