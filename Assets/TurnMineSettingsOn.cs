using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMineSettingsOn : MonoBehaviour {

    public Light FlashLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FlashLight.enabled = true;
            RenderSettings.ambientIntensity = .1f;
            RenderSettings.reflectionIntensity = 0f;
        }
    }
}
