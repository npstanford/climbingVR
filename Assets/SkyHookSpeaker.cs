using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyHookSpeaker : MonoBehaviour {
    public enum SpeakerPrograms { Intro, Climbing, Tethering, Zipshot, Glider, Battery, HeronLabs, PropellerHead, Windmill1, Windmill2, Windmill3};

    public GameObject PlayersHead;
    public AudioSource BackgroundMusic;
    public AudioSource Speaker;
    public Material SpeakerMaterial;

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

    // loading new capability
    public AudioClip CapabilityLoad;

    // climbing audio clips
    public AudioClip ClimbingExplanation;
    public AudioClip ClimbingNag;
    public AudioClip ClimbingSuccess;

    // Tethering Audio Clips
    public AudioClip TetheringExplanation;

    // zipshot audio clips
    public AudioClip ZipShotExplanation;
    public AudioClip ZipShotNag;
    public AudioClip ZipShotSuccess;

    // wingcloth audio clips
    public AudioClip WingClothExplanation;
    public AudioClip WingClothNag;
    public AudioClip WingClothSuccess;

    // battery clips
    public AudioClip BatteryExplanation;

    //propeller head audio clips
    public AudioClip PropellerHeadComputerScan;
    public AudioClip PropellerHeadExplanation;

    // windmill audio clips
    public AudioClip Static;

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
        //going to try to do a directionalized volume affect
        // take angle between speaker and players head and adjust volume based on that angle (0 degrees -> 1, 180 degrees -> 0)

        Vector3 VectorToPlayersHead = (transform.position - PlayersHead.transform.position).normalized;
        Vector3 SpeakerDirection = -transform.up.normalized;

        float angle = Mathf.Acos(Vector3.Dot(VectorToPlayersHead, SpeakerDirection));

        angle = Mathf.Clamp(angle, 0, Mathf.PI);




        //float volume = 1 - angle / Mathf.Pow(angle + 1, 2) / Mathf.Pow(Mathf.PI, 2);
        float volume = 1 - (angle / Mathf.PI);

        Speaker.volume = volume;
        BackgroundMusic.volume = volume;
	}

    public void KillAllTutorials()
    {
        StopCoroutine("Intro");
        StopCoroutine("Climbing");
        StopCoroutine("Tethering");
        StopCoroutine("Zipshot");
        StopCoroutine("WingCloth");
        StopCoroutine("PropellerHead");
        StopCoroutine("Windmill1");
    }

    public void LaunchAudio(SpeakerPrograms sp)
    {
        if (!im.EnableTutorial)
        {
            return;
        }

        KillAllTutorials();

        switch (sp)
        {
            case SpeakerPrograms.Intro:
                StartCoroutine("Intro");
                break;

            case SpeakerPrograms.Climbing:
                StartCoroutine("Climbing");
                break;

            case SpeakerPrograms.Tethering:
                StartCoroutine("Tethering");
                break;

            case SpeakerPrograms.Zipshot:
                StartCoroutine("Zipshot");
                break;

            case SpeakerPrograms.Glider:
                StartCoroutine("WingCloth");
                break;

            case SpeakerPrograms.PropellerHead:
                StartCoroutine("PropellerHead");
                break;

            case SpeakerPrograms.Windmill1:
                StartCoroutine("Windmill1");
                break;
            
            default:
                return;
        }
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
                TimeToAnnoyPlayer += ChideUserToDashStep.length + 10f;
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

    IEnumerator Climbing()
    {
        Speaker.clip = CapabilityLoad;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        BackgroundMusic.Play();

        Speaker.clip = ClimbingExplanation;
        Speaker.Play();

        im.climb.HasClimbed = false;

        while (Speaker.isPlaying && !im.climb.HasClimbed)
        {
            yield return new WaitForSeconds(.5f);
        }

        float TimeToAnnoyPlayer = Time.time + 3f;

        while (!im.climb.HasClimbed)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = ClimbingNag;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
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

    IEnumerator Tethering()
    {
        BackgroundMusic.Play();

        yield return new WaitForSeconds(5f);

        Speaker.clip = TetheringExplanation;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);

        BackgroundMusic.Stop();
    }


    IEnumerator Zipshot()
    {
        Speaker.clip = CapabilityLoad;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        BackgroundMusic.Play();

        Speaker.clip = ZipShotExplanation;
        Speaker.Play();

        im.hookshot.HasFired = false;

        while (Speaker.isPlaying && !im.hookshot.HasFired)
        {
            yield return new WaitForSeconds(.5f);
        }

        float TimeToAnnoyPlayer = Time.time + 3f;

        while (!im.hookshot.HasFired)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = ZipShotNag;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
            }
            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();
        Speaker.clip = ZipShotSuccess;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);
        BackgroundMusic.Stop();
    }

    IEnumerator WingCloth() 
    {
        Speaker.clip = CapabilityLoad;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        BackgroundMusic.Play();

        Speaker.clip = WingClothExplanation;
        Speaker.Play();

        im.glide.HasGlided = false;

        while (Speaker.isPlaying && !im.glide.HasGlided)
        {
            yield return new WaitForSeconds(.5f);
        }

        float TimeToAnnoyPlayer = Time.time + 3f;

        while (!im.glide.HasGlided)
        {
            if (Time.time > TimeToAnnoyPlayer)
            {
                Speaker.clip = WingClothNag;
                Speaker.Play();
                TimeToAnnoyPlayer += 20f;
            }
            yield return new WaitForSeconds(.5f);
        }

        Speaker.Stop();
        Speaker.clip = WingClothSuccess;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);
        BackgroundMusic.Stop();
    }

    IEnumerator BatteryRecharge()
    {
        BackgroundMusic.Play();

        yield return new WaitForSeconds(3f);

        Speaker.clip = BatteryExplanation;
        Speaker.Play();

        while (Speaker.isPlaying)
        {
            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(3f);
        BackgroundMusic.Stop();
    }

    IEnumerator PropellerHead()
    {
        Speaker.clip = PropellerHeadComputerScan;
        Speaker.Play();
        yield return new WaitForSeconds(Speaker.clip.length);

        BackgroundMusic.Play();
        yield return new WaitForSeconds(3f);
        Speaker.clip = PropellerHeadExplanation;
        Speaker.Play();
        yield return new WaitForSeconds(Speaker.clip.length);
        BackgroundMusic.Stop();
    }

    IEnumerator Windmill1()
    {
        Speaker.clip = Static;
        Speaker.Play();
        yield return null;
    }

}
