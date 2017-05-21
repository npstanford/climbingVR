using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerDestroyPHs : MonoBehaviour {

    
    public int TotalEnemies; //configurable (TODO find this on start)
    public int DestroyedEnemies;
    public float ElapsedTime;
    public Text GameStatus;

	// Use this for initialization
	void Start () {
        ElapsedTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (TotalEnemies - DestroyedEnemies > 0)
        {
            ElapsedTime = Time.time;
            GameStatus.text = ElapsedTime.ToString("F0") + "s \n" + (TotalEnemies - DestroyedEnemies) + " Remaining";
        } else
        {
            GameStatus.text = "YOU WIN! " + ElapsedTime.ToString("F0") + "s";
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered destruction field");
        Enemy e = other.gameObject.GetComponentInParent<Enemy>();
        if (e != null)
        {
            DestroyedEnemies += 1;
            CheckWinState();
            GameObject.Destroy(e.gameObject);
        }
    }

    private void CheckWinState()
    {
        if (DestroyedEnemies >= TotalEnemies)
        {
            Debug.Log("You win");
            //TODO: display text or something
        }
    }


}
