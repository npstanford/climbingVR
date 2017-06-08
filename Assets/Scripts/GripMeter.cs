using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class GripMeter : MonoBehaviour {

    //public GripManager gm;
    public float MaxGrip;
    public float RemainingGrip;
    public float GripRecoverRate;
    public float GripShakeRecoverRate;
    public float GripDepletedPenalty;
    public float FlashRate;
    public ControllerState rController;
    public ControllerState lController;


    // public bool Displayed;
    public Text LWristDisplay;
    public Text RWristDisplay;

    private Color OKColor;
    private Color BadColor;
    private Color WristColor;

    // Use this for initialization
    void Start()
    {
        RemainingGrip = MaxGrip;
        OKColor = new Color(0f, 161f, 75f, 255f);
        BadColor = Color.red;
        WristColor = OKColor;
    }

    // Update is called once per frame
    void Update()
    {

        LWristDisplay.color = WristColor;
        RWristDisplay.color = WristColor;

        if (RemainingGrip >= 0.0f)
        {
            LWristDisplay.text = RemainingGrip.ToString("F0") + " / " + MaxGrip.ToString();

            RWristDisplay.text = RemainingGrip.ToString("F0") + " / " + MaxGrip.ToString();


            if (  RemainingGrip >=  MaxGrip * .25)
            {
                WristColor = OKColor;
            } else
            {
                // I want it to flash 5 times per second
                if (Time.time % FlashRate < FlashRate / 2)
                {
                    WristColor = BadColor;

                } else
                {
                    WristColor = Color.clear;
                }
            }

             

        } else {
            LWristDisplay.text = "X";
            RWristDisplay.text = "X";
            WristColor = BadColor;
        }
    }

    public void DepleteGrip(float depletionRate)
    {
        RemainingGrip = Mathf.Max(0.0f, RemainingGrip - depletionRate * Time.deltaTime);
        if (RemainingGrip <= 0.0f) // the floating point boolean scares me a bit
        {
            DisablePlayer(GripDepletedPenalty);
        }
    }

    public void DepleteGripDiscrete(float depletionAmount)
    {
        RemainingGrip = Mathf.Max(0.0f, RemainingGrip - depletionAmount);
        if (RemainingGrip <= 0.0f) // the floating point boolean scares me a bit
        {
            DisablePlayer(GripDepletedPenalty);
        }
    }

    public void RestoreGrip(float ShakingRechargeRateL, float ShakingRechargeRateR)
    {
        RemainingGrip = Mathf.Min(MaxGrip, RemainingGrip + GripRecoverRate * Time.deltaTime);

        RemainingGrip = Mathf.Min(MaxGrip, RemainingGrip + Mathf.Max(ShakingRechargeRateL, ShakingRechargeRateR) * Time.deltaTime);
    }

    public void DisablePlayer(float penalty)
    {
        rController.ControllerShortCircuit();
        lController.ControllerShortCircuit();
        RemainingGrip = -(penalty*GripRecoverRate);
    }

}
