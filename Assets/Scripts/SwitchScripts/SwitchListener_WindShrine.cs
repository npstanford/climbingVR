using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchListener_WindShrine : MonoBehaviour {

    public SwitchControl sc;
    public Wind wind;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (sc.activated)
        {
            wind.TurnOnWind();

        } else if (!sc.activated )
        {
            wind.TurnOffWind();

        }
    }
}

