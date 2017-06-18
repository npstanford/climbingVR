using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSettingsChanger : MonoBehaviour {

    public GameObject SettingsPoint; // e.g. the point whose distance to the player determines which settings
    public Light Flashlight;

    public float MineReflection;
    public float MineAmbient;

    public float DistanceFromEntrance; //the scale we use the settings

    private RandomBackgroundNoises rbn;

    private void Start()
    {
        rbn = FindObjectOfType<RandomBackgroundNoises>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) {
            float distance = (other.transform.position - SettingsPoint.transform.position).magnitude;

            if (distance < 5f)
            {
                Flashlight.enabled = true;
                rbn.SwitchMode(RandomBackgroundNoises.BackgroundNoiseMode.Mine);
            } else
            {
                Flashlight.enabled = false;
                rbn.SwitchMode(RandomBackgroundNoises.BackgroundNoiseMode.Outside);
            }

            RenderSettings.ambientIntensity = Mathf.Lerp(MineAmbient, 1, distance / DistanceFromEntrance);
            RenderSettings.reflectionIntensity = Mathf.Lerp(MineReflection, 1, distance / DistanceFromEntrance);

        }
    }

}
