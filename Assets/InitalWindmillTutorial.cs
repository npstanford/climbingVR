using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalWindmillTutorial : MonoBehaviour {

    public SkyHookSpeaker Speaker;
    private bool Played = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !Played)
        {
            Speaker.LaunchAudio(SkyHookSpeaker.SpeakerPrograms.Windmill1);
            Played = true;
        }
    }
}
