using UnityEngine;
using System.Collections;

public class LavaController : MonoBehaviour {
    public float lavaSpeed;
    public bool lavaEnabled;
    public CanvasGroup gameOverScreen;

    public GameController gc;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (lavaEnabled)
        {
            transform.Translate(0, Time.deltaTime * lavaSpeed / 100, 0);
        }
    }

    void OnTriggerEnter()
    {
        gc.LoseGame();

    }



}
