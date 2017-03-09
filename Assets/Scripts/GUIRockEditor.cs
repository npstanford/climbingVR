using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class GUIRockEditor : MonoBehaviour
{
    public GameObject RockPrototype;
    public bool yes;

    public void Start()
    {
        RockPrototype = GameObject.FindGameObjectWithTag("RoundRock");
    }

    public void SpawnRock(Transform t)
    {
        GameObject r = GameObject.Instantiate(RockPrototype, t);
    }

}
