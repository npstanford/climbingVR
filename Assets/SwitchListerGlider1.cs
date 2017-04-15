using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchListerGlider1 : MonoBehaviour {


    public SwitchControl sc;

    public ScriptedMovement sm;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (sc.activated)
        {
            Activate();
        }
    }

    void Activate()
    {
        sm.EnableMovement = true;
    }
}
