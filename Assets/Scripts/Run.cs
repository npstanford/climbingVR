using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
public class Run : MonoBehaviour {

    public GameObject Room;
    public GameObject Player;
    public float SpeedFactor;
    public float SpeedPower;
    public float MaxSpeed;
    public Text Spedometer;
    public VignetteAndChromaticAberration EyeBlur;
    public float BlurAmount;

    public float minHeadSpeedToRun;


    public void Step(Vector3 direction, float speed)
    {
        float scaledSpeed;

        scaledSpeed = Mathf.Pow((1 + speed) * SpeedFactor, SpeedPower);
        scaledSpeed = Mathf.Min(scaledSpeed, MaxSpeed);

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;

        RaycastHit hit;
        if (Physics.Raycast(Player.transform.position, flatDirection, out hit, .15f))
        {
            InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
            if (ia != null)
            {
                if (ia.CanClimb || ia.IsGround)
                {
                    EyeBlur.intensity = 0.0f;
                    return;
                }
            }
        }

        Room.transform.localPosition += flatDirection * scaledSpeed * Time.deltaTime;
        Spedometer.text = scaledSpeed.ToString("F2");
        EyeBlur.intensity = BlurAmount * (scaledSpeed / MaxSpeed);
    }

    public void Stop()
    {
        EyeBlur.intensity = 0;
    }
}
