using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyHookSpeaker : MonoBehaviour {
    public enum SpeakerPrograms { Intro, Climbing, Zipshot, Glider, HeronLabs};
    public AudioSource BackgroundMusic;
    public AudioSource Speaker;

    // intro audio clips
    public AudioClip SkyHookCrashAndReboot;
    public AudioClip SkyHookIntro;
    public AudioClip RemindUserToPickUpTheRock;
    public AudioClip PickedUpTheRock;
    public AudioClip TellUserAboutJogging;
    public AudioClip RemindUserToJog;
    public AudioClip ExplainDashStep;
    public AudioClip ChideUserToDashStep;
    public AudioClip CongratsOnDashStep;
    public AudioClip SkyHookDisclaimer;

    // climbing audio clips
    public AudioClip ClimbingLoad;
    public AudioClip ClimbingExplanation;
    public AudioClip ClimbingNag;
    public AudioClip ClimbingSuccess;

    private InputManager im;

	// Use this for initialization
	void Start () {

        //BackgroundMusic.Play();

        im = FindObjectOfType<InputManager>();
        if (im.EnableTutorial)
        {
            StartCoroutine("Intro");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LaunchAudio(SpeakerPrograms sp)
    {
        switch (sp)
        {
            case SpeakerPrograms.Intro:
                StartCoroutine("Intro");
                break;

            case SpeakerPrograms.Climbing:
                StartCoroutine("Climbing");
                break;

            default:
                return;
        }
    }

    IEnumerator Climbing()
    {
        Speaker.clip = ClimbingLoad;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        BackgroundMusic.Play();

        Speaker.clip = ClimbingExplanation;
        Speaker.Play();

        im.climb.HasClimbed = false;

        while(Speaker.isPlaying && !im.climb.HasClimbed)
        {
            yield return new WaitForSeconds(.5f);
        }

        float TimeToAnnoyPlayer = Time.time + 10f;

        while (!im.climb.HasClimbed)
        {
            if (TimeToAnnoyPlayer > Time.time)
            {
                Speaker.clip = ClimbingNag;
                Speaker.Play();
                TimeToAnnoyPlayer += 10f;
            }
            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();
        Speaker.clip = ClimbingSuccess;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);
        BackgroundMusic.Stop();
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(5f);
        Speaker.clip = SkyHookCrashAndReboot;
        Speaker.Play();
        while(Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        BackgroundMusic.Play();

        yield return new WaitForSeconds(5f);

        im.pu.HasPickedUp = false;
        Speaker.clip = SkyHookIntro;
        Speaker.Play();
        while (Speaker.isPlaying && !im.pu.HasPickedUp)
        {
            yield return new WaitForSeconds(.5f);
        }

        float TimeToAnnoyPlayer = Time.time + 10f;

        while(!im.pu.HasPickedUp)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = RemindUserToPickUpTheRock;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
            }

            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();

        Speaker.clip = PickedUpTheRock;
        Speaker.Play();
        while(Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        im.run.HasRun = false;
        Speaker.clip = TellUserAboutJogging;
        Speaker.Play();
        while (Speaker.isPlaying && !im.run.HasRun)
        {
            yield return new WaitForSeconds(.5f);
        }

        TimeToAnnoyPlayer = Time.time + 10f;

        while (!im.run.HasRun)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = RemindUserToJog;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
            }

            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();

        im.walk.HasStepped = false;
        Speaker.clip = ExplainDashStep;
        Speaker.Play();

        while (Speaker.isPlaying && !im.walk.HasStepped)
        {
            yield return new WaitForSeconds(.5f);
        }


        TimeToAnnoyPlayer = Time.time + 10f;

        while (!im.walk.HasStepped)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = ChideUserToDashStep;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
            }

            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();
        Speaker.clip = CongratsOnDashStep;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        Speaker.clip = SkyHookDisclaimer;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);

        BackgroundMusic.Stop();

    }
}
