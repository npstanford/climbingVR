using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{

    public TeleportLocation LastTeleportLocation;
    public GameObject Player;
    private bool HaveTeleported = false;
    public SkyHookSpeaker Speaker;
    public AudioSource TeleportationSound;
    private GripMeter gm;

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GripMeter>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Teleport()
    {
        if (LastTeleportLocation != null)
        {
            StartCoroutine("TeleportCoroutine");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Teleport();
        }
    }

    IEnumerator TeleportCoroutine()
    {
        TeleportationSound.Play();
        yield return new WaitForSeconds(.5f);
        Player.transform.position = LastTeleportLocation.transform.position;
        //gm.DisablePlayer(gm.GripDepletedPenalty);
        if (!HaveTeleported)
        {
            HaveTeleported = true;
            Speaker.LaunchAudio(SkyHookSpeaker.SpeakerPrograms.Tethering);
        }
    }

}
