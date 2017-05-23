using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBallWatcher : MonoBehaviour
{
    private GoldenBall[] gbArray;
    public GameObject Door;

    // Use this for initialization
    void Start()
    {
       gbArray = FindObjectsOfType<GoldenBall>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckWinState()
    {
        foreach (GoldenBall gb in gbArray)
        {
            if (!gb.Attached)
            {
                return;
            }
        }

        //so if all balls are attached
        Win();
    }

    public void Win()
    {
        Door.SetActive(false);
    }

}