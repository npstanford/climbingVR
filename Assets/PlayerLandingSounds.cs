using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingSounds : MonoBehaviour {

    public AudioSource PlayerLandsSound;

    public void PlayerLanded()
    {
        PlayerLandsSound.Play();
    }

}
