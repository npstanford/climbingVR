using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSettingsOff : MonoBehaviour {

    public Light FlashLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FlashLight.enabled = false;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.reflectionIntensity = 1f;
        }
    }
}
