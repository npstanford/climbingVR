using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class GripMeter : MonoBehaviour {

    public GripManager gm;

    public bool Displayed;
    public Text WristDisplay;

    // Use this for initialization
    void Start()
    {
        //maybe need to pull the time and make it a timespan
    }

    // Update is called once per frame
    void Update()
    {
        WristDisplay.text = gm.GripStrength.ToString("F0") + " / " + gm.GripStrengthMax.ToString();
    }
}
